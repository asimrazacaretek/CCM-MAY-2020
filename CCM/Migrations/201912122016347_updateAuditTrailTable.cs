namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAuditTrailTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditTrails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        createdDate = c.DateTime(),
                        userId = c.String(),
                        message = c.String(),
                        stackTrace = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AuditTrails");
        }
    }
}
