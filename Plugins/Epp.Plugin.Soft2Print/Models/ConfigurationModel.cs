using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Epp.Plugin.Soft2Print.Models
{
    /// <summary>
    /// Represents a configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Soft2Print.Fields.OrganizationName")]
        public string OrganizationName { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Soft2Print.Fields.OrganizationCode")]
        public string OrganizationCode { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Soft2Print.Fields.CultureCode")]
        public string CultureCode { get; set; }

        [NopResourceDisplayName("Plugins.Misc.Soft2Print.Fields.CustomerId")]
        public string CustomerId { get; set; }

        //[NopResourceDisplayName("Plugins.Misc.Soft2Print.Fields.Testing")]
        //[Range(1,3)]
        //public int Testing { get; set; }
        [NopResourceDisplayName("Plugins.Misc.Soft2Print.Fields.ProjectNameFormat")]
        public string ProjectNameFormat { get; set; } = "Proj_{CustomerId}_{ProjectId}";

        public bool HideGeneralBlock { get; set; }

        #endregion
    }
}