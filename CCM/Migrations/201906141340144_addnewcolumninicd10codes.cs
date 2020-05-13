namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewcolumninicd10codes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Icd10Codes", "DiseaseState", c => c.String());
            AddColumn("dbo.Icd10Codes", "DiseaseType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Icd10Codes", "DiseaseType");
            DropColumn("dbo.Icd10Codes", "DiseaseState");
        }
    }
}
