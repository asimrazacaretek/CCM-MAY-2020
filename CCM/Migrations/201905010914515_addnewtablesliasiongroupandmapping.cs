namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtablesliasiongroupandmapping : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LiaisonGroup_Liaison_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LiaisonId = c.Int(nullable: false),
                        LiaisonGroupId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Liaisons", t => t.LiaisonId, cascadeDelete: true)
                .ForeignKey("dbo.LiaisonGroups", t => t.LiaisonGroupId, cascadeDelete: true)
                .Index(t => t.LiaisonId)
                .Index(t => t.LiaisonGroupId);
            
            CreateTable(
                "dbo.LiaisonGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupName = c.String(nullable: false, maxLength: 100, unicode: false),
                        MainPhoneNumber = c.String(nullable: false),
                        Email = c.String(nullable: false, maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LiaisonGroup_Liaison_Mapping", "LiaisonGroupId", "dbo.LiaisonGroups");
            DropForeignKey("dbo.LiaisonGroup_Liaison_Mapping", "LiaisonId", "dbo.Liaisons");
            DropIndex("dbo.LiaisonGroup_Liaison_Mapping", new[] { "LiaisonGroupId" });
            DropIndex("dbo.LiaisonGroup_Liaison_Mapping", new[] { "LiaisonId" });
            DropTable("dbo.LiaisonGroups");
            DropTable("dbo.LiaisonGroup_Liaison_Mapping");
        }
    }
}
