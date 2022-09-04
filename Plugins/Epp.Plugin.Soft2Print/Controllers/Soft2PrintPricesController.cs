using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Epp.Plugin.Soft2Print.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Services.Orders;
using Nop.Services.Catalog;
using Nop.Plugin.Soft2Print.Services;
using Nop.Services.Directory;

namespace Epp.Plugin.Soft2Print.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class Soft2PrintPricesController : BasePluginController
    {
        #region Fields

        private readonly IEmailAccountService _emailAccountService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly MessageTemplatesSettings _messageTemplatesSettings;
        private readonly Soft2PrintSettings _settings;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IS2POrdersService _s2pOrdersService;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly IProductAttributeService _productAttributeService;
        #endregion

        #region Ctor

        public Soft2PrintPricesController(IEmailAccountService emailAccountService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            INotificationService notificationService,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IWorkContext workContext,
              IShoppingCartService shoppingCartService,
            IProductService productService,
            IS2POrdersService s2pOrdersService,
               ICategoryService categoryService,
            MessageTemplatesSettings messageTemplatesSettings,
            Soft2PrintSettings settings,
            ICurrencyService currencyService,
            IProductAttributeService productAttributeService)
        {
            _emailAccountService = emailAccountService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _messageTemplateService = messageTemplateService;
            _messageTokenProvider = messageTokenProvider;
            _notificationService = notificationService;
            _settingService = settingService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _s2pOrdersService = s2pOrdersService;
            _categoryService = categoryService;
            _workContext = workContext;
            _messageTemplatesSettings = messageTemplatesSettings;
            _settings = settings;
            _currencyService = currencyService;
            _productAttributeService = productAttributeService;
        }

        #endregion

        #region Methods

        public async Task<ActionResult> Index(string call, string id, string product, int quantity, int additionalSheetCount)
        {
            await _logger.InformationAsync($"Soft2PrintPrices called: call={call}, id={id}, product={product}, quantity={quantity}, additionalSheetCount={additionalSheetCount}");
            if (call == "GetFormatStrings")
            {
                Nop.Core.Domain.Directory.Currency culture = null;
                /* if (!string.IsNullOrEmpty(id))
                 {
                     int currencyId = 0;
                     if (!int.TryParse(id, out currencyId))
                     {
                         culture = await _currencyService.GetCurrencyByIdAsync(currencyId);
                     }
                 }
                 else
                 {
                     culture = await _workContext.GetWorkingCurrencyAsync();
                 }*/
                culture = await _workContext.GetWorkingCurrencyAsync();
                await _logger.ErrorAsync($"culture{culture.DisplayLocale} ");
                double amount = 0;
                var formattedString = amount.ToString("C", System.Globalization.CultureInfo.GetCultureInfo(culture.DisplayLocale));
                var ret = "{0:" + formattedString + "; ; FREE}";
                return Content(ret);
            }
            else if (call == "GetPrice")
            {
                var price = "";
                var productPrice = 0.00;
                var additionalSheetPrice = 0.00;
                var totalCustomPrice = 0.00;
                int prodId = 0;
                if (int.TryParse(product.Trim(), out prodId))
                {
                    var productObj = await _productService.GetProductByIdAsync(prodId);
                    productPrice = Convert.ToDouble(productObj?.Price);
                    //price = productObj.Price.ToString("0.00");
                }
                if (Convert.ToInt32(additionalSheetCount) > 0)
                {
                    var requiredAttributeNames = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(Convert.ToInt32(prodId));
                    if (requiredAttributeNames.Count > 0)
                    {
                        var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(requiredAttributeNames[0].ProductAttributeId);

                        if (productAttribute.Name.Equals("Additional Sheet Price", StringComparison.OrdinalIgnoreCase))
                        {
                            additionalSheetPrice = Convert.ToDouble(requiredAttributeNames[0].DefaultValue);
                        }
                    }
                    totalCustomPrice = (productPrice * quantity) + (Convert.ToInt32(additionalSheetCount) * Convert.ToDouble(additionalSheetPrice));
                }
                else
                {
                    totalCustomPrice = productPrice * quantity;
                }

                price = totalCustomPrice.ToString("0.00");
                await _logger.InformationAsync($"Price = {price} ");
                return Content(price);
            }
            else if (call == "GetPrices")
            {
                var price = "";
                foreach (var prod in product.Split(';'))
                {
                    int prodId = 0;
                    if (int.TryParse(prod.Trim(), out prodId))
                    {
                        var productObj = await _productService.GetProductByIdAsync(prodId);
                        price += prodId.ToString() + ":" + productObj.Price.ToString("0.00") + ";";
                    }
                    //  await _logger.ErrorAsync($"Price = {price} ");
                }
            }
            return Content("");
        }

        #endregion
    }
}