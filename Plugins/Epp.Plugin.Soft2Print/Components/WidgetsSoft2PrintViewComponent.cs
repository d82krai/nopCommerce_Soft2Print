using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Web.Framework.Components;

namespace Epp.Plugin.Soft2Print.Components
{
    /// <summary>
    /// Represents view component to embed tracking script on pages
    /// </summary>
    [ViewComponent(Name = Soft2PrintDefaults.TRACKING_VIEW_COMPONENT_NAME)]
    public class Soft2PrintViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly Soft2PrintSettings _soft2printSettings;

        #endregion

        #region Ctor

        public Soft2PrintViewComponent(ICustomerService customerService,
            IWorkContext workContext,
            Soft2PrintSettings soft2printSettings)
        {
            _customerService = customerService;
            _workContext = workContext;
            _soft2printSettings = soft2printSettings;
        }

        #endregion

        #region Methods

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
            var trackingScript = string.Empty;

            //ensure Marketing Automation is enabled
            if (!_soft2printSettings.UseMarketingAutomation)
                return Content(trackingScript);

            //get customer email
            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerEmail = !await _customerService.IsGuestAsync(customer)
                ? customer.Email?.Replace("'", "\\'")
                : string.Empty;

            //prepare tracking script
            trackingScript = $"{_soft2printSettings.TrackingScript}{Environment.NewLine}"
                .Replace(Soft2PrintDefaults.TrackingScriptId, _soft2printSettings.MarketingAutomationKey)
                .Replace(Soft2PrintDefaults.TrackingScriptCustomerEmail, customerEmail);

            return View("~/Plugins/Misc.Soft2Print/Views/PublicInfo.cshtml", trackingScript);
        }

        #endregion
    }
}