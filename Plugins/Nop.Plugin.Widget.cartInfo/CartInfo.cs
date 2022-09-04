using System.Threading.Tasks;
using Nop.Services.Plugins;

namespace Nop.Plugin.Widget.cartInfo
{
    public class CartInfoPlugin : BasePlugin
    {
        public override async Task InstallAsync()
        {
            //Logic during installation goes here...

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            //Logic during uninstallation goes here...

            await base.UninstallAsync();
        }
    }
}