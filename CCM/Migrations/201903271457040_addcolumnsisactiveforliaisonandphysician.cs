namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnsisactiveforliaisonandphysician : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Liaisons", "isActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Physicians", "isActive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Physicians", "isActive");
            DropColumn("dbo.Liaisons", "isActive");
        }
    }
}
