namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtableMedicationRoute : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MedicationRoutes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RouteItme = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MedicationRoutes");
        }
    }
}
