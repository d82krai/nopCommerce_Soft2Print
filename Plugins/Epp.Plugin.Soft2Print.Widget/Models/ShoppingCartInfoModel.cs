using Nop.Web.Framework.Models;

namespace Epp.Plugin.Soft2Print.Widget.Models
{
    /// <summary>
    /// Represents a shopping cart info model
    /// </summary>
    public record ShoppingCartInfoModel : BaseNopModel
    {
        #region Properties

        public int CartId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string AdditionalSheetCount { get; set; }

        #endregion
    }
}