using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Configuration;
using Soft2Print;

namespace Epp.Plugin.Soft2Print
{
    public class Soft2PrintApi
    {
        private readonly ISettingService _settingService;        
        private readonly IStoreContext _storeContext;

        public Soft2PrintApi(ISettingService settingService,
            IStoreContext storeContext)
        {
            _settingService = settingService;
                _storeContext = storeContext;
        }
        public async Task<string> GetGuid(string customerId)
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var soft2PrintSettings = await _settingService.LoadSettingAsync<Soft2PrintSettings>(storeScope);

            string orgName = soft2PrintSettings.OrganizationName;
            string orgCode = soft2PrintSettings.OrganizationCode;
            string culture = soft2PrintSettings.CultureCode;
            Organization_API_v4SoapClient client = new Organization_API_v4SoapClient(Organization_API_v4SoapClient.EndpointConfiguration.Organization_API_v4Soap);
            return await client.ValidateUserAsync(orgName, orgCode, culture, customerId);
        }

        public async Task<int> GetProjectId(string guid, int customerId, int productId)
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var soft2PrintSettings = await _settingService.LoadSettingAsync<Soft2PrintSettings>(storeScope);

            string orgName = soft2PrintSettings.OrganizationName; 
            string orgCode = soft2PrintSettings.OrganizationCode;
            var projectName = soft2PrintSettings.ProjectNameFormat;
            projectName = projectName.Replace("{ProductId}", productId.ToString());
            projectName = projectName.Replace("{CustomerId}", customerId.ToString());
            projectName += DateTime.UtcNow.ToString("_yyMMdd_") + Guid.NewGuid().ToString().Substring(0, 4);
            Organization_API_v4SoapClient client = new Organization_API_v4SoapClient(Organization_API_v4SoapClient.EndpointConfiguration.Organization_API_v4Soap);
            return await client.UserProject_CreateAsync(orgName, orgCode, guid, projectName);
        }
    }
}
