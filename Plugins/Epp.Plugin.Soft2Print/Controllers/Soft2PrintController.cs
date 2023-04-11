using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Epp.Plugin.Soft2Print.Models;
using Epp.Plugin.Soft2Print.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Nop.Services.Orders;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Plugin.Soft2Print.Services;
using Epp.Plugin.Soft2Print.Domain;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;

namespace Epp.Plugin.Soft2Print.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class Soft2PrintController : BasePluginController
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
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductModelFactory _productModelFactory;
        #endregion

        #region Ctor

        public Soft2PrintController(IEmailAccountService emailAccountService,
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
             IProductAttributeFormatter productAttributeFormatter,
            IProductModelFactory productModelFactory,
            IS2POrdersService s2pOrdersService,
               ICategoryService categoryService,
                 IProductAttributeService productAttributeService,
            MessageTemplatesSettings messageTemplatesSettings,
            Soft2PrintSettings settings)
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
            _productAttributeService = productAttributeService;
            _productAttributeFormatter = productAttributeFormatter;
            _productModelFactory = productModelFactory;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _s2pOrdersService = s2pOrdersService;
            _categoryService = categoryService;
            _workContext = workContext;
            _messageTemplatesSettings = messageTemplatesSettings;
            _settings = settings;

        }

        #endregion
               

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            /* if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                 return AccessDeniedView();*/

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var soft2PrintSettings = await _settingService.LoadSettingAsync<Soft2PrintSettings>(storeScope);

            var model = new ConfigurationModel
            {
                OrganizationName = soft2PrintSettings.OrganizationName,
                OrganizationCode = soft2PrintSettings.OrganizationCode,
                CultureCode = soft2PrintSettings.CultureCode,
                CustomerId = soft2PrintSettings.CustomerId,
                //Testing = soft2PrintSettings.Testing,
                ProjectNameFormat = soft2PrintSettings.ProjectNameFormat,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                soft2PrintSettings.OrganizationName = model.OrganizationName;
                soft2PrintSettings.OrganizationCode = model.OrganizationCode;
                soft2PrintSettings.CultureCode = model.CultureCode;
                soft2PrintSettings.CustomerId = model.CustomerId;
                //    soft2PrintSettings.Testing = model.Testing;
                soft2PrintSettings.ProjectNameFormat = model.ProjectNameFormat;
            }

            return View("~/Plugins/Misc.Soft2Print/Views/Configure.cshtml", model);

        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var soft2PrintSettings = await _settingService.LoadSettingAsync<Soft2PrintSettings>(storeId);

            //set API key
            soft2PrintSettings.OrganizationName = model.OrganizationName;
            soft2PrintSettings.OrganizationCode = model.OrganizationCode;
            soft2PrintSettings.CultureCode = model.CultureCode;
            soft2PrintSettings.CustomerId = model.CustomerId;
            //   soft2PrintSettings.Testing = model.Testing;
            soft2PrintSettings.ProjectNameFormat = model.ProjectNameFormat;

            // await _settingService.SaveSettingAsync(soft2PrintSettings, settings => settings.ApiKey, clearCache: false);
            /* We do not clear cache after each setting update.
            * This behavior can increase performance because cached settings will not be cleared 
            * and loaded from database after each update */
            await _settingService.SaveSettingAsync(soft2PrintSettings, x => x.OrganizationName, storeId, false);
            await _settingService.SaveSettingAsync(soft2PrintSettings, x => x.OrganizationCode, storeId, false);
            await _settingService.SaveSettingAsync(soft2PrintSettings, x => x.CultureCode, storeId, false);
            await _settingService.SaveSettingAsync(soft2PrintSettings, x => x.CustomerId, storeId, false);
            await _settingService.SaveSettingAsync(soft2PrintSettings, x => x.Testing, storeId, false);
            await _settingService.SaveSettingAsync(soft2PrintSettings, x => x.ProjectNameFormat, storeId, false);
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [Authorize]
        public async Task<IActionResult> Design(int productId)
        {
            //Checking if proudct is alredy added to Cart then do not allow adding the same product
            var alreadyAddedToCart = await IsProductAlreadyAddedToCart(productId);
            if (alreadyAddedToCart)
            {
                await _logger.InformationAsync($"ProductId={productId} can not be added to Cart again, as it is already added. And it is soft2print product.");
                return RedirectToRoute("Homepage");
            }

            DesignModel model = new DesignModel();
            ProductAttributeCombinationModel modelProduct = new ProductAttributeCombinationModel();

            //Call ValidateUser api : output= guid;
            Soft2PrintApi api = new Soft2PrintApi(_settingService, _storeContext);
            var customer = await _workContext.GetCurrentCustomerAsync();
            var custGUID = await api.GetGuid(customer.Id.ToString());
            var projectId = await api.GetProjectId(custGUID.ToString(), customer.Id, productId);


            //create S2P record in table
            await _s2pOrdersService.InsertS2POrderRecordAsync(new S2POrders
            {
                JobId = 0,
                StoreId = 0,
                CustomerId = customer.Id,
                ProductId = productId,
                CreatedDate = DateTime.Now,
                ProjectId = projectId,
                AdditionalSheetCount = "0",
                Status = Domain.OrderStatus.Design,
                OrderId = 0,
                ShoppingCartId = 0,
                AdditionalSheetPrice = 0
            });
            //Call UserProject_Create api : output=project id
            model.ProjectId = projectId.ToString();
            model.CustGUID = custGUID.ToString();

            var categoryMapping = (await _categoryService.GetProductCategoriesByProductIdAsync(productId)).FirstOrDefault();
            var categoryName = (await _categoryService.GetCategoryByIdAsync(categoryMapping?.CategoryId ?? 0))?.Name;
            
            model.Module = categoryName;        // "Gifting";
            model.ProductId = productId.ToString();
            model.Url = $@"//services.soft2print.com/module.aspx?id={model.CustGUID}&module={model.Module}&Productcode={model.ProductId}&ProjectID={model.ProjectId}";
            await _logger.InformationAsync($"ProductId={productId} , URL = {model.Url}, categoryName = {categoryName}");
            return View("~/Plugins/Misc.Soft2Print/Views/Design.cshtml", model);
            // Response.Redirect($@"//services.soft2print.com/module.aspx?id={model.CustGUID}&module={model.Module}&Productcode={model.ProductId}&ProjectID={model.ProjectId}");
            //return View();
        }

        private async Task<bool> IsProductAlreadyAddedToCart(int productId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            return cart.Any(m => m.ProductId == productId);
        }

        //[Authorize]
        public async Task<IActionResult> AddToCart(string id, string jobid, string productcode, string additionalSheetCount)
        {
            await _logger.InformationAsync($"Add to cart function productid = {productcode},additionalSheetCount = {additionalSheetCount}");
            decimal totalCustomPrice;
            string additionalStyleSheet = "0";
            string additionalSheetCountPrice = "0";
            var product = await _productService.GetProductByIdAsync(Convert.ToInt32(productcode));
            await _logger.InformationAsync($"GetProductByIdAsync productid = {productcode},additionalSheetCount = {additionalSheetCount}");
            var customer = await _workContext.GetCurrentCustomerAsync();
            await _logger.InformationAsync($" GetCurrentCustomerAsyncproductid = {productcode},additionalSheetCount =  {additionalSheetCount}");
            var store = await _storeContext.GetCurrentStoreAsync();
            await _logger.InformationAsync($"GetCurrentStoreAsync productid = {productcode},additionalSheetCount =  {additionalSheetCount}");
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            await _logger.InformationAsync($"GetShoppingCartAsync productid = {productcode},additionalSheetCount =  {additionalSheetCount}");
            if (Convert.ToInt32(additionalSheetCount) > 0)
            {
                var requiredAttributeNames = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(Convert.ToInt32(productcode));
                if (requiredAttributeNames.Count > 0)
                {
                    var productAttribute = await _productAttributeService.GetProductAttributeByIdAsync(requiredAttributeNames[0].ProductAttributeId);

                    if (productAttribute.Name.Equals("Additional Sheet Price", StringComparison.OrdinalIgnoreCase))
                    {
                        additionalStyleSheet = requiredAttributeNames[0].DefaultValue;
                    }
                }
                var productPrice = product.Price;
                additionalSheetCountPrice = additionalStyleSheet;
                totalCustomPrice = productPrice + (Convert.ToInt32(additionalSheetCount) * Convert.ToInt32(additionalSheetCountPrice));
            }
            else
            {
                totalCustomPrice = product.Price;
            }
        /*    if (Convert.ToInt32(additionalSheetCount) > 0)
                product.Name = product.Name + "(" + additionalSheetCount + ")";*/
            //now let's try adding product to the cart (now including product attribute validation, etc)
            var addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: customer,
                product: product,
                shoppingCartType: ShoppingCartType.ShoppingCart,
                storeId: store.Id, string.Empty, totalCustomPrice, null, null, 1, true);

            cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id, Convert.ToInt32(productcode));
            var newCartItem = cart.FirstOrDefault();
            await _logger.InformationAsync($"productid = {productcode},newcartitem = {newCartItem.ProductId}");
            if (newCartItem != null)
            {
                //update remaining fie_s2pOrdersServicelds of s2pOrdersrecord
                var s2p = await _s2pOrdersService.FindRecordsAsync(customer.Id, Convert.ToInt32(productcode), null, null, Domain.OrderStatus.Design, null);
                if (s2p != null)
                {
                    s2p.StoreId = store.Id;
                    s2p.AdditionalSheetCount = additionalSheetCount;
                    s2p.JobId = Convert.ToInt32(jobid);
                    s2p.Status = Domain.OrderStatus.AddedToCart;
                    s2p.ShoppingCartId = newCartItem.Id;
                    s2p.AdditionalSheetPrice = Convert.ToDecimal(additionalSheetCountPrice);
                    await _s2pOrdersService.UpdateS2POrderRecordAsync(s2p);
                }
            }
            //return View("~/Plugins/Misc.Soft2Print/Views/AddToCart.cshtml");
            return RedirectToRoute("ShoppingCart");
        }

        #endregion
    }
}