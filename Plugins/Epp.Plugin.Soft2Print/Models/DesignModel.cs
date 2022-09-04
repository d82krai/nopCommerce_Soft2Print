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
    public record DesignModel : BaseNopModel
    {

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }
               
        public string CustGUID { get; set; }
            
        public string Module { get; set; }

        public string ProductId { get; set; }

        public string ProjectId { get; set; }

        public string Url { get; set; }
        #endregion
    }
}