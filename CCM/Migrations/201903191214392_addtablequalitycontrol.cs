namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtablequalitycontrol : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QualityControls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 50, unicode: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        FirstName = c.String(nullable: false, maxLength: 100, unicode: false),
                        LastName = c.String(nullable: false, maxLength: 100, unicode: false),
                        Gender = c.String(maxLength: 50, unicode: false),
                        MobilePhoneNumber = c.String(nullable: false),
                        Email = c.String(nullable: false, maxLength: 100, unicode: false),
                        Address = c.String(maxLength: 100, unicode: false),
                        City = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.QualityControls");
        }
    }
}
