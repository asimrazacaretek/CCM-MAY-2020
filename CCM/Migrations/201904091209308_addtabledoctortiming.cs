namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtabledoctortiming : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorTimings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        PhysicianID = c.Int(),
                        WeekDayName = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        ClinicID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Physicians", t => t.PhysicianID)
                .Index(t => t.PhysicianID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DoctorTimings", "PhysicianID", "dbo.Physicians");
            DropIndex("dbo.DoctorTimings", new[] { "PhysicianID" });
            DropTable("dbo.DoctorTimings");
        }
    }
}
