using Epp.Plugin.Soft2Print.Domain;
using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;


namespace Nop.Plugin.Soft2Print.Data
{
    [NopMigration("2022/06/20 08:40:55:1687541", "S2POrders base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<S2POrders>();
        }
    }
}