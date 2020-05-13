namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addphotoinphysician : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Physicians", "Photo", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Physicians", "Photo");
        }
    }
}
