namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CCMCycleStatusRejectionHistorytable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CCMCycleStatusRejectionHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        CCMStatus = c.String(maxLength: 50, unicode: false),
                        Cycle = c.Int(nullable: false),
                        rejectionDate = c.DateTime(),
                        SubmittedBy = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CCMCycleStatusRejectionHistories");
        }
    }
}
