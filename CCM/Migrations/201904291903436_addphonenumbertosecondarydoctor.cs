namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addphonenumbertosecondarydoctor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SecondaryDoctors", "MobilePhoneNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SecondaryDoctors", "MobilePhoneNumber");
        }
    }
}
