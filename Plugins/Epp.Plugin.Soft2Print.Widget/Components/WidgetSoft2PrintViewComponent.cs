using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using Nop.Services.Catalog;
using Nop.Plugin.Soft2Print.Services;
using Nop.Core;

namespace Epp.Plugin.Soft2Print.Widget.Components
{

    public class WidgetSoft2PrintViewComponent : NopViewComponent
    {
        #region Fields
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IS2POrdersService _s2pOrdersService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public WidgetSoft2PrintViewComponent(IProductAttributeService productAttributeService,
            IProductService productService,
            IS2POrdersService s2pOrdersService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _productAttributeService = productAttributeService;
            _productService = productService;
            _s2pOrdersService = s2pOrdersService;
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
            return View("~/Plugins/Misc.Soft2Print.Widget/Views/CartView.cshtml");
        }

    }
}
