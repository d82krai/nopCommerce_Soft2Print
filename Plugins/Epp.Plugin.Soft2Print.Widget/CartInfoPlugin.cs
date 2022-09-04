using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Epp.Plugin.Soft2Print.Widget
{
    public class CartInfoPlugin : BasePlugin,IWidgetPlugin
    {
        public bool HideInWidgetList => false;

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetSoft2Print";
        }

        public async  Task<IList<string>> GetWidgetZonesAsync()
        {
            return new List<string> { PublicWidgetZones.OrderSummaryContentAfter };
        }

        public override Task InstallAsync()
        {
            return base.InstallAsync();
        }
        public override Task UninstallAsync()
        {
            return base.UninstallAsync();
        }
    }
}
