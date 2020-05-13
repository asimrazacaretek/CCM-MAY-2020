namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_TicketAttachmentClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketAttachments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        userTicketId = c.String(),
                        filePath = c.String(),
                        fileName = c.String(),
                        createdBy = c.String(),
                        createdDate = c.DateTime(),
                        updatedBy = c.String(),
                        updatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TicketAttachments");
        }
    }
}
