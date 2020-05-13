namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnreceivernameincareplansharedhistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CarePlanSharedHistories", "ReceiverName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CarePlanSharedHistories", "ReceiverName");
        }
    }
}
