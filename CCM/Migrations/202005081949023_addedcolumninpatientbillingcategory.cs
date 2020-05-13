namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedcolumninpatientbillingcategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients_BillingCategories", "TranslatorId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients_BillingCategories", "TranslatorId");
        }
    }
}
