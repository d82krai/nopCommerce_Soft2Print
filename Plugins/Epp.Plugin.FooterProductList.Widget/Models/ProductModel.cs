using Nop.Web.Framework.Models;

namespace Epp.Plugin.FooterProductList.Widget.Models
{
    /// <summary>
    /// Represents a shopping cart info model
    /// </summary>
    public record ProductModel : BaseNopModel
    {
        #region Properties

      
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductURL { get; set; }

        #endregion
    }
}