namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnstobillingcycles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillingCycles", "BillingCode1PhysicianRateInvoice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.BillingCycles", "BillingCode2PhysicianRateInvoice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.BillingCycles", "BillingCode1PhysicianRateBilling", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.BillingCycles", "BillingCode2PhysicianRateBilling", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BillingCycles", "BillingCode2PhysicianRateBilling");
            DropColumn("dbo.BillingCycles", "BillingCode1PhysicianRateBilling");
            DropColumn("dbo.BillingCycles", "BillingCode2PhysicianRateInvoice");
            DropColumn("dbo.BillingCycles", "BillingCode1PhysicianRateInvoice");
        }
    }
}
