using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using Nop.Services.Orders;
using Nop.Core.Domain.Orders;
using Nop.Web.Models.ShoppingCart;
using Nop.Web.Factories;
using System.Collections.Generic;
using System;
using Nop.Services.Logging;
using Epp.Plugin.FooterProductList.Widget.Models;
using Nop.Services.Catalog;
using Nop.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Seo;

namespace Epp.Plugin.FooterProductList.Widget.Controllers
{
    public class FooterProductListWidgetController : BasePluginController
    {
        #region Fields
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly ILogger _logger;
        protected readonly IRepository<Product> _productRepository;
        private readonly ICategoryService _categoryService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        #endregion

        #region Ctor

        public FooterProductListWidgetController(IShoppingCartModelFactory shoppingCartModelFactory,
            IStoreContext storeContext,
            IWorkContext workContext,
              IShoppingCartService shoppingCartService,
            IProductService productService,
            ILogger logger,
            ICategoryService categoryService,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            CatalogSettings catalogSettings,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper)
        {
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _productService = productService;
            _workContext = workContext;
            _logger = logger;
            _categoryService = categoryService;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _categoryService = categoryService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
        }

        #endregion


        #region Methods        

        [HttpGet]
        public async Task<JsonResult> GetProductList()
        {        
           // var productList = await _productService.GetAssociatedProductsAsync(1);

            var model = new List<ProductModel>();
            var store = await _storeContext.GetCurrentStoreAsync();
            var storeId = store.Id;
            var categories = _categoryService.GetAllCategoriesAsync(storeId);
            await _logger.InformationAsync($"categories called: ={categories.Result.Count}");
            IList<int> cat = new List<int>();   
            for(var i=0;i<categories.Result.Count ;i++)
            {
                cat.Add(categories.Result[i].Id);
            }
            
            var products = await _productService.SearchProductsAsync(storeId: storeId);
            
            await _logger.InformationAsync($"products called: ={products.Count}");
            foreach (var item in products)
            {
                var productUrl = Url.RouteUrl("Product", new { SeName = await _urlRecordService.GetSeNameAsync(item) }, _webHelper.GetCurrentRequestProtocol());
                model.Add(new ProductModel()
                {
                    ProductName = item.Name,
                    ProductURL = productUrl
                }) ;
            }
           
            /* for(int i=0;i<10;i++)
              {
                  model.Add(new ProductModel()
                  {
                      ProductName = "product" + i
                  });
              }*/
            await _logger.InformationAsync($"productList called: ={model.Count}");
            return Json(model);
        }

        #endregion
    }
}