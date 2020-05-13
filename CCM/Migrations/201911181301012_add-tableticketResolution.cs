namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtableticketResolution : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketResolutions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        resolutionName = c.String(),
                        CreatedBy = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        updatedBy = c.String(),
                        updatedDate = c.DateTime(),
                        isDeleted = c.Boolean(nullable: false),
                        deletedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TicketResolutions");
        }
    }
}
