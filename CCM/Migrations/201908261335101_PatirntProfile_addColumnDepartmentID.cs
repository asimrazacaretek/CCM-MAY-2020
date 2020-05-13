namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PatirntProfile_addColumnDepartmentID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientProfile_HospitalDetails", "DepartmentId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientProfile_HospitalDetails", "DepartmentId");
        }
    }
}
