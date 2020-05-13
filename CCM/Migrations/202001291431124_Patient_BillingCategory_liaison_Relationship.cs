namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patient_BillingCategory_liaison_Relationship : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients_BillingCategories", "LiaisonId", c => c.Int());
            CreateIndex("dbo.Patients_BillingCategories", "LiaisonId");
            AddForeignKey("dbo.Patients_BillingCategories", "LiaisonId", "dbo.Liaisons", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients_BillingCategories", "LiaisonId", "dbo.Liaisons");
            DropIndex("dbo.Patients_BillingCategories", new[] { "LiaisonId" });
            DropColumn("dbo.Patients_BillingCategories", "LiaisonId");
        }
    }
}
