using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Epp.Plugin.FooterProductList.Widget
{
    public class ProductListPlugin : BasePlugin,IWidgetPlugin
    {
        public bool HideInWidgetList => false;

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetFooterProductList";
        }

        public async  Task<IList<string>> GetWidgetZonesAsync()
        {
            return new List<string> { PublicWidgetZones.Footer };
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
