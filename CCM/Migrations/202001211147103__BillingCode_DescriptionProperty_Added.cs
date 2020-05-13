namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _BillingCode_DescriptionProperty_Added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillingCodes", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BillingCodes", "Description");
        }
    }
}
