namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnstopatient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "PhotoEmrRecords7", c => c.Binary());
            AddColumn("dbo.Patients", "PhotoEmrRecords8", c => c.Binary());
            AddColumn("dbo.Patients", "PhotoEmrRecords9", c => c.Binary());
            AddColumn("dbo.Patients", "PhotoEmrRecords10", c => c.Binary());
            AddColumn("dbo.Patients", "PhotoEmrRecords11", c => c.Binary());
            AddColumn("dbo.Patients", "PhotoEmrRecords12", c => c.Binary());
            AddColumn("dbo.Patients", "PhotoEmrRecords13", c => c.Binary());
            AddColumn("dbo.Patients", "PhotoEmrRecords14", c => c.Binary());
            AddColumn("dbo.Patients", "PhotoEmrRecords15", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients", "PhotoEmrRecords15");
            DropColumn("dbo.Patients", "PhotoEmrRecords14");
            DropColumn("dbo.Patients", "PhotoEmrRecords13");
            DropColumn("dbo.Patients", "PhotoEmrRecords12");
            DropColumn("dbo.Patients", "PhotoEmrRecords11");
            DropColumn("dbo.Patients", "PhotoEmrRecords10");
            DropColumn("dbo.Patients", "PhotoEmrRecords9");
            DropColumn("dbo.Patients", "PhotoEmrRecords8");
            DropColumn("dbo.Patients", "PhotoEmrRecords7");
        }
    }
}
