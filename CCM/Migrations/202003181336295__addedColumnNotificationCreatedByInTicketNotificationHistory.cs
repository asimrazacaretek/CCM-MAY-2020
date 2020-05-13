namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _addedColumnNotificationCreatedByInTicketNotificationHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketNotificationHistories", "NotificationCreatedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketNotificationHistories", "NotificationCreatedBy");
        }
    }
}
