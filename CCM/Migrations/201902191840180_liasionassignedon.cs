namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class liasionassignedon : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "LiasionAssignedOn", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients", "LiasionAssignedOn");
        }
    }
}
