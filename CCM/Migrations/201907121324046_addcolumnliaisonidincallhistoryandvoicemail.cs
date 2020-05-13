namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnliaisonidincallhistoryandvoicemail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CallHistories", "LiaisonId", c => c.Int(nullable: false));
            AddColumn("dbo.VoiceMailHistories", "LiaisonId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VoiceMailHistories", "LiaisonId");
            DropColumn("dbo.CallHistories", "LiaisonId");
        }
    }
}
