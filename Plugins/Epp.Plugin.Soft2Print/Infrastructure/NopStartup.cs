using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Epp.Plugin.Soft2Print.Services;
using Nop.Services.Messages;
using Nop.Plugin.Soft2Print.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Epp.Plugin.Soft2Print.ViewEngines;
using Nop.Services.Orders;

namespace Epp.Plugin.Soft2Print.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring services on application startup
    /// </summary>
    public class NopStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //register custom services
            services.AddScoped<Soft2PrintManager>();
            services.AddScoped<Soft2PrintManager>();
            services.AddScoped<IS2POrdersService, S2POrdersService>();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CustomViewEngine());
            });

            services.AddScoped<IOrderTotalCalculationService, PriceCalculationService>();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 3000;
    }
}