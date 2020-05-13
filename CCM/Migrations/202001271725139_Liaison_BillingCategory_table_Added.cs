namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Liaison_BillingCategory_table_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Liaisons_BillingCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BillingCategoryId = c.Int(),
                        LiaisonId = c.Int(),
                        Status = c.Boolean(nullable: false),
                        CycleId = c.Int(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        EnrolledOn = c.DateTime(),
                        DeEnrolledOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BillingCategories", t => t.BillingCategoryId)
                .ForeignKey("dbo.Liaisons", t => t.LiaisonId)
                .Index(t => t.BillingCategoryId)
                .Index(t => t.LiaisonId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Liaisons_BillingCategories", "LiaisonId", "dbo.Liaisons");
            DropForeignKey("dbo.Liaisons_BillingCategories", "BillingCategoryId", "dbo.BillingCategories");
            DropIndex("dbo.Liaisons_BillingCategories", new[] { "LiaisonId" });
            DropIndex("dbo.Liaisons_BillingCategories", new[] { "BillingCategoryId" });
            DropTable("dbo.Liaisons_BillingCategories");
        }
    }
}
