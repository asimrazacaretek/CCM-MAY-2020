namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewcolumnsDoctorVisitAndSecondaryDoctor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorVisits", "Email", c => c.String());
            AddColumn("dbo.SecondaryDoctors", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SecondaryDoctors", "Email");
            DropColumn("dbo.DoctorVisits", "Email");
        }
    }
}
