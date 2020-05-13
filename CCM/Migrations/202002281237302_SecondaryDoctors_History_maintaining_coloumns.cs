namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SecondaryDoctors_History_maintaining_coloumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SecondaryDoctors", "Status", c => c.Boolean());
            AddColumn("dbo.SecondaryDoctors", "CreatedOn", c => c.DateTime());
            AddColumn("dbo.SecondaryDoctors", "CreatedBy", c => c.String());
            AddColumn("dbo.SecondaryDoctors", "UpdatedOn", c => c.DateTime());
            AddColumn("dbo.SecondaryDoctors", "UpdatedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SecondaryDoctors", "UpdatedBy");
            DropColumn("dbo.SecondaryDoctors", "UpdatedOn");
            DropColumn("dbo.SecondaryDoctors", "CreatedBy");
            DropColumn("dbo.SecondaryDoctors", "CreatedOn");
            DropColumn("dbo.SecondaryDoctors", "Status");
        }
    }
}
