using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core.Infrastructure;
using Nop.Web.Framework;
using Nop.Web.Framework.Themes;

namespace Epp.Plugin.Soft2Print.ViewEngines
{
    public class CustomViewEngine : IViewLocationExpander
    {
        private const string THEME_KEY = "nop.themename";

        /// <summary>
        /// Invoked by a Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine to determine the
        /// values that would be consumed by this instance of Microsoft.AspNetCore.Mvc.Razor.IViewLocationExpander.
        /// The calculated values are used to determine if the view location has changed since the last time it was located.
        /// </summary>
        /// <param name="context">Context</param>
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //no need to add the themeable view locations at all as the administration should not be themeable anyway
            if (context.AreaName?.Equals(AreaNames.Admin) ?? false)
                return;

            context.Values[THEME_KEY] = EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync().Result;
        }

        /// <summary>
        /// Invoked by a Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine to determine potential locations for a view.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="viewLocations">View locations</param>
        /// <returns>iew locations</returns>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.Values.TryGetValue(THEME_KEY, out string theme))
            {
                /*if (context.ViewName == "Components/OrderSummary/Default")
                {
                    viewLocations = new[] {
                        $"/Plugins/Misc.Soft2Print/Themes/{theme}/Views/Shared/Components/OrderSummary/Default.cshtml",
                    }
                        .Concat(viewLocations);
                }
                else
                {*/
                    viewLocations = new[] {
                        $"/Plugins/Misc.Soft2Print/Views/{{0}}.cshtml",
                        $"/Plugins/Misc.Soft2Print/Views/Product/{{0}}.cshtml",
                        $"/Plugins/Misc.Soft2Print/Themes/{theme}/Views/Shared/Components/{{1}}/{{0}}.cshtml",
                        $"/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                        $"/Themes/{theme}/Views/Shared/{{0}}.cshtml",
                    }
                        .Concat(viewLocations);
               // }
            }
            
            return viewLocations;
        }
    }
}
