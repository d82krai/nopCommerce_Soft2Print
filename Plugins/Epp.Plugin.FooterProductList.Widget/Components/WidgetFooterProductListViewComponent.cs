using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Services.Catalog;
using Nop.Core;

namespace Epp.Plugin.FooterProductList.Widget.Components
{

    public class WidgetFooterProductListViewComponent : NopViewComponent
    {
        #region Fields
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WidgetFooterProductListViewComponent(IProductAttributeService productAttributeService,
            IProductService productService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _productAttributeService = productAttributeService;
            _productService = productService;
            _storeContext = storeContext;
            _workContext = workContext;
        }
        #endregion 

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {           
            return View("~/Plugins/Misc.FooterProductList.Widget/Views/ProductList.cshtml");
        }

    }
}
