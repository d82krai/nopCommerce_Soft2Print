using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Epp.Plugin.Soft2Print.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(Soft2PrintDefaults.AddToCartRoute, "Design/AddToCart",
               new { controller = "Soft2Print", action = "AddToCart" });
            endpointRouteBuilder.MapControllerRoute(Soft2PrintDefaults.DesignRoute, $"Design/Design/{{productId:min(0)}}",
              new { controller = "Soft2Print", action = "Design" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}