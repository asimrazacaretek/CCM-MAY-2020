namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class G0506patientupdatetable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.G0506_PatientsInfo", "Status", c => c.Boolean(nullable: false));
            AddColumn("dbo.G0506_PatientsInfo", "CreatedBy", c => c.String());
            AddColumn("dbo.G0506_PatientsInfo", "CreatedOn", c => c.DateTime());
            AddColumn("dbo.G0506_PatientsInfo", "UpdatedBy", c => c.String());
            AddColumn("dbo.G0506_PatientsInfo", "UpdatedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.G0506_PatientsInfo", "UpdatedOn");
            DropColumn("dbo.G0506_PatientsInfo", "UpdatedBy");
            DropColumn("dbo.G0506_PatientsInfo", "CreatedOn");
            DropColumn("dbo.G0506_PatientsInfo", "CreatedBy");
            DropColumn("dbo.G0506_PatientsInfo", "Status");
        }
    }
}
