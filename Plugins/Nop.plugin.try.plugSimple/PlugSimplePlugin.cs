using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.plugin.FirstTry.PlugSimple
{
    internal class PlugSimplePlugin : BasePlugin,IWidgetPlugin
    {
        public bool HideInWidgetList => false;

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "FirstSimple";
        }

        public async Task<IList<string>> GetWidgetZonesAsync()
        {
            return new List<string> {  PublicWidgetZones.HomepageBeforeNews };
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
