namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewtablesphysicianandliasoncptrates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Liaison_CPTRates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        LiaisonId = c.Int(nullable: false),
                        BillingCode = c.String(maxLength: 100, unicode: false),
                        SalaryRate = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Physician_CPTRates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(),
                        CreatedBy = c.String(maxLength: 100, unicode: false),
                        UpdatedOn = c.DateTime(),
                        UpdatedBy = c.String(maxLength: 100, unicode: false),
                        PhysicianId = c.Int(nullable: false),
                        BillingCode = c.String(maxLength: 100, unicode: false),
                        InvoiceRate = c.Decimal(precision: 18, scale: 2),
                        BillingRate = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Liaisons", "Liaison_CPTRates_Id", c => c.Int());
            AddColumn("dbo.Physicians", "Physician_CPTRates_Id", c => c.Int());
            CreateIndex("dbo.Liaisons", "Liaison_CPTRates_Id");
            CreateIndex("dbo.Physicians", "Physician_CPTRates_Id");
            AddForeignKey("dbo.Liaisons", "Liaison_CPTRates_Id", "dbo.Liaison_CPTRates", "Id");
            AddForeignKey("dbo.Physicians", "Physician_CPTRates_Id", "dbo.Physician_CPTRates", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Physicians", "Physician_CPTRates_Id", "dbo.Physician_CPTRates");
            DropForeignKey("dbo.Liaisons", "Liaison_CPTRates_Id", "dbo.Liaison_CPTRates");
            DropIndex("dbo.Physicians", new[] { "Physician_CPTRates_Id" });
            DropIndex("dbo.Liaisons", new[] { "Liaison_CPTRates_Id" });
            DropColumn("dbo.Physicians", "Physician_CPTRates_Id");
            DropColumn("dbo.Liaisons", "Liaison_CPTRates_Id");
            DropTable("dbo.Physician_CPTRates");
            DropTable("dbo.Liaison_CPTRates");
        }
    }
}
