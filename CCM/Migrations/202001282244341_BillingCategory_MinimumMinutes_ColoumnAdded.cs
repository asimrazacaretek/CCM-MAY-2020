namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BillingCategory_MinimumMinutes_ColoumnAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillingCategories", "MinimunMinutes", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BillingCategories", "MinimunMinutes");
        }
    }
}
