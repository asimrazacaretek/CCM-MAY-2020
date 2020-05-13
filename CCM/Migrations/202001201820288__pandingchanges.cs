namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _pandingchanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Liaison_CPTRates", "LiaisonId", "dbo.Liaisons");
            DropForeignKey("dbo.Physician_CPTRates", "PhysicianId", "dbo.Physicians");
            DropIndex("dbo.Liaison_CPTRates", new[] { "LiaisonId" });
            DropIndex("dbo.Physician_CPTRates", new[] { "PhysicianId" });
            AlterColumn("dbo.Liaison_CPTRates", "LiaisonId", c => c.Int());
            AlterColumn("dbo.Physician_CPTRates", "PhysicianId", c => c.Int());
            CreateIndex("dbo.Liaison_CPTRates", "LiaisonId");
            CreateIndex("dbo.Physician_CPTRates", "PhysicianId");
            AddForeignKey("dbo.Liaison_CPTRates", "LiaisonId", "dbo.Liaisons", "Id");
            AddForeignKey("dbo.Physician_CPTRates", "PhysicianId", "dbo.Physicians", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Physician_CPTRates", "PhysicianId", "dbo.Physicians");
            DropForeignKey("dbo.Liaison_CPTRates", "LiaisonId", "dbo.Liaisons");
            DropIndex("dbo.Physician_CPTRates", new[] { "PhysicianId" });
            DropIndex("dbo.Liaison_CPTRates", new[] { "LiaisonId" });
            AlterColumn("dbo.Physician_CPTRates", "PhysicianId", c => c.Int(nullable: false));
            AlterColumn("dbo.Liaison_CPTRates", "LiaisonId", c => c.Int(nullable: false));
            CreateIndex("dbo.Physician_CPTRates", "PhysicianId");
            CreateIndex("dbo.Liaison_CPTRates", "LiaisonId");
            AddForeignKey("dbo.Physician_CPTRates", "PhysicianId", "dbo.Physicians", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Liaison_CPTRates", "LiaisonId", "dbo.Liaisons", "Id", cascadeDelete: true);
        }
    }
}
