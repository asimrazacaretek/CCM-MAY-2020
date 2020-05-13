namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnsBillingRatestoBillingCycle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillingCycles", "BillingCode1Rate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.BillingCycles", "BillingCode2Rate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BillingCycles", "BillingCode2Rate");
            DropColumn("dbo.BillingCycles", "BillingCode1Rate");
        }
    }
}
