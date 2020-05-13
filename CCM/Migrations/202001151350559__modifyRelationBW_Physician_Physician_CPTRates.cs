namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _modifyRelationBW_Physician_Physician_CPTRates : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Physicians", "Physician_CPTRates_Id", "dbo.Physician_CPTRates");
            DropIndex("dbo.Physicians", new[] { "Physician_CPTRates_Id" });
            CreateIndex("dbo.Physician_CPTRates", "PhysicianId");
            AddForeignKey("dbo.Physician_CPTRates", "PhysicianId", "dbo.Physicians", "Id", cascadeDelete: true);
            DropColumn("dbo.Physicians", "Physician_CPTRates_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Physicians", "Physician_CPTRates_Id", c => c.Int());
            DropForeignKey("dbo.Physician_CPTRates", "PhysicianId", "dbo.Physicians");
            DropIndex("dbo.Physician_CPTRates", new[] { "PhysicianId" });
            CreateIndex("dbo.Physicians", "Physician_CPTRates_Id");
            AddForeignKey("dbo.Physicians", "Physician_CPTRates_Id", "dbo.Physician_CPTRates", "Id");
        }
    }
}
