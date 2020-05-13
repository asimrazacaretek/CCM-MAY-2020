namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtablereasons : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reasons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReasonText = c.String(),
                        Module = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Reasons");
        }
    }
}
