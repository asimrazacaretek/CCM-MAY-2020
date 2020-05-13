namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumntypeupdatebygroupchats : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GroupChats", "UpdatedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GroupChats", "UpdatedOn", c => c.DateTime(nullable: false));
        }
    }
}
