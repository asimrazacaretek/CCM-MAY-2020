namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnenrolleridindoctortiming : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorTimings", "EnrollerID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DoctorTimings", "EnrollerID");
        }
    }
}
