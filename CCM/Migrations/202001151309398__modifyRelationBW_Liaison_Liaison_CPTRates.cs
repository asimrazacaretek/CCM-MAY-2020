namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _modifyRelationBW_Liaison_Liaison_CPTRates : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Liaisons", "Liaison_CPTRates_Id", "dbo.Liaison_CPTRates");
            DropIndex("dbo.Liaisons", new[] { "Liaison_CPTRates_Id" });
            CreateIndex("dbo.Liaison_CPTRates", "LiaisonId");
            AddForeignKey("dbo.Liaison_CPTRates", "LiaisonId", "dbo.Liaisons", "Id", cascadeDelete: true);
            DropColumn("dbo.Liaisons", "Liaison_CPTRates_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Liaisons", "Liaison_CPTRates_Id", c => c.Int());
            DropForeignKey("dbo.Liaison_CPTRates", "LiaisonId", "dbo.Liaisons");
            DropIndex("dbo.Liaison_CPTRates", new[] { "LiaisonId" });
            CreateIndex("dbo.Liaisons", "Liaison_CPTRates_Id");
            AddForeignKey("dbo.Liaisons", "Liaison_CPTRates_Id", "dbo.Liaison_CPTRates", "Id");
        }
    }
}
