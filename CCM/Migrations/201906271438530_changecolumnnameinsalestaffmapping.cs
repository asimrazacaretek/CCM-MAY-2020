namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumnnameinsalestaffmapping : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PhysicianGroup_SalesStaff_Mapping", "SaleStaff_Id", "dbo.SaleStaffs");
            DropIndex("dbo.PhysicianGroup_SalesStaff_Mapping", new[] { "SaleStaff_Id" });
            RenameColumn(table: "dbo.PhysicianGroup_SalesStaff_Mapping", name: "SaleStaff_Id", newName: "SaleStaffId");
            AlterColumn("dbo.PhysicianGroup_SalesStaff_Mapping", "SaleStaffId", c => c.Int(nullable: false));
            CreateIndex("dbo.PhysicianGroup_SalesStaff_Mapping", "SaleStaffId");
            AddForeignKey("dbo.PhysicianGroup_SalesStaff_Mapping", "SaleStaffId", "dbo.SaleStaffs", "Id", cascadeDelete: true);
            DropColumn("dbo.PhysicianGroup_SalesStaff_Mapping", "SalesStaffId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PhysicianGroup_SalesStaff_Mapping", "SalesStaffId", c => c.Int(nullable: false));
            DropForeignKey("dbo.PhysicianGroup_SalesStaff_Mapping", "SaleStaffId", "dbo.SaleStaffs");
            DropIndex("dbo.PhysicianGroup_SalesStaff_Mapping", new[] { "SaleStaffId" });
            AlterColumn("dbo.PhysicianGroup_SalesStaff_Mapping", "SaleStaffId", c => c.Int());
            RenameColumn(table: "dbo.PhysicianGroup_SalesStaff_Mapping", name: "SaleStaffId", newName: "SaleStaff_Id");
            CreateIndex("dbo.PhysicianGroup_SalesStaff_Mapping", "SaleStaff_Id");
            AddForeignKey("dbo.PhysicianGroup_SalesStaff_Mapping", "SaleStaff_Id", "dbo.SaleStaffs", "Id");
        }
    }
}
