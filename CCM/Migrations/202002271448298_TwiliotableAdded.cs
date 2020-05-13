namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TwiliotableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TwilioNumbersTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FriendlyPhoneNumer = c.String(),
                        MobilePhoneNumber = c.String(),
                        Status = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Liaisons", "TwilioNumbersTableId", c => c.Int());
            CreateIndex("dbo.Liaisons", "TwilioNumbersTableId");
            AddForeignKey("dbo.Liaisons", "TwilioNumbersTableId", "dbo.TwilioNumbersTables", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Liaisons", "TwilioNumbersTableId", "dbo.TwilioNumbersTables");
            DropIndex("dbo.Liaisons", new[] { "TwilioNumbersTableId" });
            DropColumn("dbo.Liaisons", "TwilioNumbersTableId");
            DropTable("dbo.TwilioNumbersTables");
        }
    }
}
