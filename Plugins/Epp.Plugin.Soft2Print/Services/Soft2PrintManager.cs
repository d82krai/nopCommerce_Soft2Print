using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Soft2Print.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using SendinblueMarketingAutomation.Api;
using SendinblueMarketingAutomation.Client;
using SendinblueMarketingAutomation.Model;
using Soft2Print;

namespace Epp.Plugin.Soft2Print.Services
{
    /// <summary>
    /// Represents soft2print manager
    /// </summary>
    public class Soft2PrintManager
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IAddressService _addressService;
        private readonly ICategoryService _categoryService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly Soft2PrintSettings _soft2printSettings;
        private readonly ISettingService _settingService;
        private readonly IS2POrdersService _s2pOrdersService;
        #endregion

        #region Ctor

        public Soft2PrintManager(CurrencySettings currencySettings,
            IActionContextAccessor actionContextAccessor,
            IAddressService addressService,
            ICategoryService categoryService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILogger logger,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPictureService pictureService,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            Soft2PrintSettings soft2printSettings,
            ISettingService settingService,
            IS2POrdersService s2pOrdersService,
            IHttpContextAccessor httpContextAccessor)
        {
            _currencySettings = currencySettings;
            _actionContextAccessor = actionContextAccessor;
            _addressService = addressService;
            _categoryService = categoryService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _logger = logger;
            _orderService = orderService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _pictureService = pictureService;
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
            _s2pOrdersService = s2pOrdersService;
            _webHelper = webHelper;
            _workContext = workContext;
            _soft2printSettings = soft2printSettings;
            _settingService = settingService;
            _httpContextAccessor = httpContextAccessor;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle order paid event
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleOrderPaidEventAsync(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var soft2PrintSettings = await _settingService.LoadSettingAsync<Soft2PrintSettings>(storeScope);

            string orgName = soft2PrintSettings.OrganizationName;
            string orgCode = soft2PrintSettings.OrganizationCode;
            Soft2PrintApi api = new Soft2PrintApi(_settingService, _storeContext);

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            var nopGUID = await api.GetGuid(customer.Id.ToString());
            var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0);
            var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
            //order.CustomerCurrencyCode
            var currency = _workContext.GetWorkingCurrencyAsync().Result.CurrencyCode;
            Organization_API_v4SoapClient client1 = new Organization_API_v4SoapClient(Organization_API_v4SoapClient.EndpointConfiguration.Organization_API_v4Soap);
            var orderCreateResult = await client1.Order_CreateAsync(orgName, orgCode, nopGUID, order.Id.ToString(), currency, billingAddress.Company, "", billingAddress.FirstName, billingAddress.LastName,
                        billingAddress.Address1, billingAddress.Address2, billingAddress.ZipPostalCode, billingAddress.City,
                        (await _countryService.GetCountryByAddressAsync(billingAddress))?.Name, billingAddress.PhoneNumber, "", "", billingAddress.Email,
                        shippingAddress.Company, "", shippingAddress.FirstName, shippingAddress.LastName, shippingAddress.Address1,
                        shippingAddress.Address2, shippingAddress.ZipPostalCode, shippingAddress.City,
                        (await _countryService.GetCountryByAddressAsync(shippingAddress))?.Name);
            
            string jobid = "";
            var orderItems = (await _orderService.GetOrderItemsAsync(order.Id));
            foreach (var item in orderItems)
            {
                //get jobid
                var s2p = await _s2pOrdersService.FindRecordsAsync(customer.Id, Convert.ToInt32(item.ProductId), null, null, Domain.OrderStatus.AddedToCart,null);
                if (s2p != null)
                {
                    jobid = s2p.JobId.ToString();
                }
                //if productcategory is customized then
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                var categoryMapping = (await _categoryService.GetProductCategoriesByProductIdAsync(product?.Id ?? 0)).FirstOrDefault();
                //var categoryName = (await _categoryService.GetCategoryByIdAsync(categoryMapping?.CategoryId ?? 0))?.Name;
                var category = (await _categoryService.GetCategoryByIdAsync(categoryMapping?.CategoryId ?? 0));
                var parentCategoryName = (await _categoryService.GetCategoryByIdAsync(category?.ParentCategoryId ?? 0))?.Name;
             
                if (parentCategoryName == "Customizable Products")
                {
                    var orderAddJobResult = await client1.Order_AddJobAsync(orgName, orgCode, order.Id.ToString(), jobid, item.Quantity,
                        item.PriceInclTax, 0, "", 0, "", true);
                }
                if (s2p != null)
                {
                    s2p.Status = Domain.OrderStatus.OrderPaid;
                    s2p.OrderId = order.Id;
                    await _s2pOrdersService.UpdateS2POrderRecordAsync(s2p);
                }
            }
            await client1.Order_ConfirmAsync(orgName, orgCode, order.Id.ToString());
        }
        #endregion
    }
}