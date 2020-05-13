namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnsinsecondarydoctors : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SecondaryDoctors", "NPI", c => c.Int());
            AddColumn("dbo.SecondaryDoctors", "MainPhoneNumber", c => c.String(nullable: false));
            AddColumn("dbo.SecondaryDoctors", "Address1", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.SecondaryDoctors", "Address2", c => c.String(maxLength: 100, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SecondaryDoctors", "Address2");
            DropColumn("dbo.SecondaryDoctors", "Address1");
            DropColumn("dbo.SecondaryDoctors", "MainPhoneNumber");
            DropColumn("dbo.SecondaryDoctors", "NPI");
        }
    }
}
