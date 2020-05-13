namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumnname : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DoctorTimings", "PhysicianID", "dbo.Physicians");
            DropIndex("dbo.DoctorTimings", new[] { "PhysicianID" });
            AddColumn("dbo.DoctorTimings", "LiaisonID", c => c.Int());
            CreateIndex("dbo.DoctorTimings", "LiaisonID");
            AddForeignKey("dbo.DoctorTimings", "LiaisonID", "dbo.Liaisons", "Id");
            DropColumn("dbo.DoctorTimings", "PhysicianID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DoctorTimings", "PhysicianID", c => c.Int());
            DropForeignKey("dbo.DoctorTimings", "LiaisonID", "dbo.Liaisons");
            DropIndex("dbo.DoctorTimings", new[] { "LiaisonID" });
            DropColumn("dbo.DoctorTimings", "LiaisonID");
            CreateIndex("dbo.DoctorTimings", "PhysicianID");
            AddForeignKey("dbo.DoctorTimings", "PhysicianID", "dbo.Physicians", "Id");
        }
    }
}
