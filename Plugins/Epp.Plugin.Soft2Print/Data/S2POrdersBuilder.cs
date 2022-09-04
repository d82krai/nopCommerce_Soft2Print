using Epp.Plugin.Soft2Print.Domain;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;


namespace Nop.Plugin.Soft2Print.Mapping.Builders
{
    public class S2POrdersBuilder : NopEntityBuilder<S2POrders>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                 .WithColumn(nameof(S2POrders.JobId)).AsInt32()
                 .WithColumn(nameof(S2POrders.StoreId)).AsInt32()
                 .WithColumn(nameof(S2POrders.CustomerId)).AsInt32()
                 .WithColumn(nameof(S2POrders.ProductId)).AsInt32()
                 .WithColumn(nameof(S2POrders.CreatedDate)).AsDateTime()
                 .WithColumn(nameof(S2POrders.ProjectId)).AsInt32()
                .WithColumn(nameof(S2POrders.AdditionalSheetCount))
                .AsString(3)
                .WithColumn(nameof(S2POrders.Status)).AsInt32()
                .Nullable()
                .WithColumn(nameof(S2POrders.OrderId)).AsInt32()
                .WithColumn(nameof(S2POrders.ShoppingCartId)).AsInt32()
                .WithColumn(nameof(S2POrders.AdditionalSheetPrice)).AsDecimal(18, 4);
        }
    }
}