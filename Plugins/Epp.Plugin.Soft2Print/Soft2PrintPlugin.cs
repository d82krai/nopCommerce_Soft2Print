using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Services.Stores;
using Nop.Web.Framework.Infrastructure;

namespace Epp.Plugin.Soft2Print
{
    /// <summary>
    /// Represents the soft2print plugin
    /// </summary>
    public class Soft2PrintPlugin : BasePlugin, IMiscPlugin, IWidgetPlugin
    {
        #region Fields

        private readonly IEmailAccountService _emailAccountService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWebHelper _webHelper;
        private readonly WidgetSettings _widgetSettings;
        private readonly IStoreContext _storeContext;
        #endregion

        #region Ctor

        public Soft2PrintPlugin(IEmailAccountService emailAccountService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IMessageTemplateService messageTemplateService,
            IScheduleTaskService scheduleTaskService,
            ISettingService settingService,
            IStoreService storeService,
            IWebHelper webHelper,
            WidgetSettings widgetSettings,IStoreContext storeContext)
        {
            _emailAccountService = emailAccountService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _messageTemplateService = messageTemplateService;
            _scheduleTaskService = scheduleTaskService;
            _settingService = settingService;
            _storeService = storeService;
            _webHelper = webHelper;
            _widgetSettings = widgetSettings;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget zones
        /// </returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.ProductBoxAddinfoMiddle });
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return Soft2PrintDefaults.TRACKING_VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Soft2Print/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new Soft2PrintSettings
            {
                //prepopulate a tracking script
                TrackingScript = $@"<!-- Sendinblue tracking code -->
                <script>
                    (function() {{
                        window.sib = {{ equeue: [], client_key: '{Soft2PrintDefaults.TrackingScriptId}' }};
                        window.sib.email_id = '{Soft2PrintDefaults.TrackingScriptCustomerEmail}';
                        window.sendinblue = {{}}; for (var j = ['track', 'identify', 'trackLink', 'page'], i = 0; i < j.length; i++) {{ (function(k) {{ window.sendinblue[k] = function() {{ var arg = Array.prototype.slice.call(arguments); (window.sib[k] || function() {{ var t = {{}}; t[k] = arg; window.sib.equeue.push(t);}})(arg[0], arg[1], arg[2]);}};}})(j[i]);}}var n = document.createElement('script'),i = document.getElementsByTagName('script')[0]; n.type = 'text/javascript', n.id = 'sendinblue-js', n.async = !0, n.src = 'https://sibautomation.com/sa.js?key=' + window.sib.client_key, i.parentNode.insertBefore(n, i), window.sendinblue.page();
                    }})();
                </script>"
            });

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(Soft2PrintDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(Soft2PrintDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            //install synchronization task
            if (await _scheduleTaskService.GetTaskByTypeAsync(Soft2PrintDefaults.SynchronizationTask) == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
                {
                    Enabled = true,
                    LastEnabledUtc = DateTime.UtcNow,
                    Seconds = Soft2PrintDefaults.DefaultSynchronizationPeriod * 60 * 60,
                    Name = Soft2PrintDefaults.SynchronizationTaskName,
                    Type = Soft2PrintDefaults.SynchronizationTask,
                });
            }

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Misc.Soft2Print.Fields.OrganizationName"] = "Organization Name",
                ["Plugins.Misc.Soft2Print.Fields.OrganizationName.Hint"] = "Organization Name received from Soft2Print",
                ["Plugins.Misc.Soft2Print.Fields.OrganizationCode"] = "Organization Code",
                ["Plugins.Misc.Soft2Print.Fields.OrganizationCode.Hint"] = "Organization Code received from Soft2Print",
                ["Plugins.Misc.Soft2Print.Fields.CultureCode"] = "Culture Code",
                ["Plugins.Misc.Soft2Print.Fields.CultureCode.Hint"] = "e.g. en-US, en-UK",
                ["Plugins.Misc.Soft2Print.Fields.CustomerId"] = "Customer Id",
                ["Plugins.Misc.Soft2Print.Fields.CustomerId.Hint"] = "Soft2Print Customer Id, not nopCommerce customerId.",
                ["Plugins.Misc.Soft2Print.Fields.Testing"] = "Testing?",
                ["Plugins.Misc.Soft2Print.Fields.Testing.Hint"] = "At the time of placing order this flag will be shared with Soft2Print.",
                ["Plugins.Misc.Soft2Print.Fields.ProjectNameFormat"] = "Project Name Format",
                ["Plugins.Misc.Soft2Print.Fields.ProjectNameFormat.Hint"] = "e.g. ProjName_{ProductId}_{CustomerId}. Supported Variables: ProductId, CustomerId. Plugin will automatically add 10 characters long string at the end, which is combination of date and 4 random characters, e.g. <yyMMdd????>",
                ["Plugins.Epp.Plugin.Soft2Print.Instructions"] = "Soft2Print plugin help your customer to use the <a href='https://demo.soft2print.com/' target='_blank'>MediaClip</a>, so that they can design the product their own.<br><br> " +
                        "<b>Step 1:</b><br> " +
                        "Update below url in soft2print's website configuration in Settings -> UrlSettings <br> " +
                        "In URL Packges settings - Go to URL's section and update below url's <br> " +
                        "1. Update Add to carts URL to '" + _storeContext.GetCurrentStoreAsync().Result.Url + "Design/AddToCart' <br> " +
                        "2. Update Price URL to '" + _storeContext.GetCurrentStoreAsync().Result.Url + "Soft2PrintPrices' <br> " +
                        "<b>Step 2:</b><br> " +
                        "To configure Soft2Print plugin please enter below details and click Save button. <br>"+
                        "<b>Step 3:</b><br> " +
                        "In Soft2Print website go to Settings -> Security. And provide below information.<br > " +
                        "Name: Your Server Name<br> " +
                        "IP: Your Server IP <br>" +
                        "<b>Step 4:</b><br> " +
                        "In nopCommerce create top level category called 'Customizable Products' and then add the sub-category under 'Customizable Products' for each Soft2Print modules(e.g. Calendar, Gifting, GreetingCard, PhotoBook... Then set the product inside each sub-categories."
                        
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //smtp accounts
            foreach (var store in await _storeService.GetAllStoresAsync())
            {
                var key = $"{nameof(Soft2PrintSettings)}.{nameof(Soft2PrintSettings.EmailAccountId)}";
                var emailAccountId = await _settingService.GetSettingByKeyAsync<int>(key, storeId: store.Id, loadSharedValueIfNotFound: true);
                var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(emailAccountId);
                if (emailAccount != null)
                    await _emailAccountService.DeleteEmailAccountAsync(emailAccount);
            }

            //settings
            if (_widgetSettings.ActiveWidgetSystemNames.Contains(Soft2PrintDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Remove(Soft2PrintDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }
            await _settingService.DeleteSettingAsync<Soft2PrintSettings>();

            //generic attributes
            foreach (var store in await _storeService.GetAllStoresAsync())
            {
                var messageTemplates = await _messageTemplateService.GetAllMessageTemplatesAsync(store.Id);
                foreach (var messageTemplate in messageTemplates)
                {
                    await _genericAttributeService.SaveAttributeAsync<int?>(messageTemplate, Soft2PrintDefaults.TemplateIdAttribute, null);
                }
            }

            //schedule task
            var task = await _scheduleTaskService.GetTaskByTypeAsync(Soft2PrintDefaults.SynchronizationTask);
            if (task != null)
                await _scheduleTaskService.DeleteTaskAsync(task);

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.Soft2Print");

            await base.UninstallAsync();
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => true;
    }
}
