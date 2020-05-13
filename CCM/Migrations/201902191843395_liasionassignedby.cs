namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class liasionassignedby : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "LiasionAssignedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients", "LiasionAssignedBy");
        }
    }
}
