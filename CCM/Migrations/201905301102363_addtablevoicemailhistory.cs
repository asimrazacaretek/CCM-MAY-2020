namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtablevoicemailhistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VoiceMailHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        To = c.String(),
                        From = c.String(),
                        Status = c.String(),
                        StartTime = c.DateTime(),
                        EndTime = c.DateTime(),
                        Duration = c.Time(nullable: false, precision: 7),
                        RecordingURL = c.String(),
                        CallerId = c.String(),
                        TwilioCallId = c.String(),
                        PatientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VoiceMailHistories");
        }
    }
}
