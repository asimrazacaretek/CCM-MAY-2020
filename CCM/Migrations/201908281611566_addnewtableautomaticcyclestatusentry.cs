namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtableautomaticcyclestatusentry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AutomaticCycleStatusEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EntryMonth = c.Int(nullable: false),
                        EntryYear = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AutomaticCycleStatusEntries");
        }
    }
}
