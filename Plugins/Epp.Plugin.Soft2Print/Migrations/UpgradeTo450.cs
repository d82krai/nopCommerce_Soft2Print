using Epp.Plugin.Soft2Print.Domain;
using FluentMigrator;
using Nop.Data.Migrations;


namespace Nop.Plugin.Soft2Print.Migrations
{
    [NopMigration("2022/06/20 08:40:55", "S2POrders change decimal precision", MigrationProcessType.Update)]
    public class ChangeDecimalPrecision : Migration
    {
        public override void Up()
        {
                    
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
