namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EnrollmentStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EnrollmentSubstatusReasons",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EnrollmentSubStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100, unicode: false),
                        EnrollmentStatusID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EnrollmentStatus", t => t.EnrollmentStatusID, cascadeDelete: true)
                .Index(t => t.EnrollmentStatusID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EnrollmentSubStatus", "EnrollmentStatusID", "dbo.EnrollmentStatus");
            DropIndex("dbo.EnrollmentSubStatus", new[] { "EnrollmentStatusID" });
            DropTable("dbo.EnrollmentSubStatus");
            DropTable("dbo.EnrollmentSubstatusReasons");
            DropTable("dbo.EnrollmentStatus");
        }
    }
}
