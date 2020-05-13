namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnspatienttablesubstatusandreason : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "EnrollmentSubStatus", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.Patients", "EnrollmentSubStatusReason", c => c.String(maxLength: 100, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients", "EnrollmentSubStatusReason");
            DropColumn("dbo.Patients", "EnrollmentSubStatus");
        }
    }
}
