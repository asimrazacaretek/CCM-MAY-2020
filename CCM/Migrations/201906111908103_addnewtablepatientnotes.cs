namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtablepatientnotes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatientNotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Notes = c.String(),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                        Module = c.String(maxLength: 100),
                        PatientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PatientNotes");
        }
    }
}
