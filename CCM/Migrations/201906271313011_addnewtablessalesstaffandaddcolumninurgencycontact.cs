namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtablessalesstaffandaddcolumninurgencycontact : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PhysicianGroup_SalesStaff_Mapping",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SalesStaffId = c.Int(nullable: false),
                        PhysiciansGroupId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        SaleStaff_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PhysiciansGroups", t => t.PhysiciansGroupId, cascadeDelete: true)
                .ForeignKey("dbo.SaleStaffs", t => t.SaleStaff_Id)
                .Index(t => t.PhysiciansGroupId)
                .Index(t => t.SaleStaff_Id);
            
            CreateTable(
                "dbo.SaleStaffs",
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
            
            AddColumn("dbo.PatientProfile_UrgencyContact", "ContactType", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PhysicianGroup_SalesStaff_Mapping", "SaleStaff_Id", "dbo.SaleStaffs");
            DropForeignKey("dbo.PhysicianGroup_SalesStaff_Mapping", "PhysiciansGroupId", "dbo.PhysiciansGroups");
            DropIndex("dbo.PhysicianGroup_SalesStaff_Mapping", new[] { "SaleStaff_Id" });
            DropIndex("dbo.PhysicianGroup_SalesStaff_Mapping", new[] { "PhysiciansGroupId" });
            DropColumn("dbo.PatientProfile_UrgencyContact", "ContactType");
            DropTable("dbo.SaleStaffs");
            DropTable("dbo.PhysicianGroup_SalesStaff_Mapping");
        }
    }
}
