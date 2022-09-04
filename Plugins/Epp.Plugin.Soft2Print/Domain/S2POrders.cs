using System;
using Epp.Plugin.Soft2Print.Domain;
using Nop.Core;

namespace Epp.Plugin.Soft2Print.Domain
{
    /// <summary>
    /// Represents a shipping by weight record
    /// </summary>
    public partial class S2POrders : BaseEntity
    {
       /* /// <summary>
        /// Gets or sets the S2POrder identifier
        /// </summary>
        public int Id { get; set; }*/

        /// <summary>
        /// Gets or sets the Job id
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }
            
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the created date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the soft2print project id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the AdditionalSheetCount value
        /// </summary>
        public string AdditionalSheetCount { get; set; }

        /// <summary>
        /// Gets or sets the "Order status" value
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the OrderId
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the ShoppingCartId
        /// </summary>
        public int ShoppingCartId { get; set; }

        /// <summary>
        /// Gets or sets the OrderId
        /// </summary>
        public decimal AdditionalSheetPrice { get; set; }
    }
}