namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnsreceiveridandreceivertypeincareplansharedhistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CarePlanSharedHistories", "ReceiverId", c => c.Int(nullable: false));
            AddColumn("dbo.CarePlanSharedHistories", "ReceiverType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CarePlanSharedHistories", "ReceiverType");
            DropColumn("dbo.CarePlanSharedHistories", "ReceiverId");
        }
    }
}
