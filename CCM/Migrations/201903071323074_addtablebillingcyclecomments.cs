namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtablebillingcyclecomments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BillingCycleComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BillingCycleId = c.Int(nullable: false),
                        Comments = c.String(),
                        Status = c.String(maxLength: 50, unicode: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BillingCycles", t => t.BillingCycleId, cascadeDelete: true)
                .Index(t => t.BillingCycleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BillingCycleComments", "BillingCycleId", "dbo.BillingCycles");
            DropIndex("dbo.BillingCycleComments", new[] { "BillingCycleId" });
            DropTable("dbo.BillingCycleComments");
        }
    }
}
