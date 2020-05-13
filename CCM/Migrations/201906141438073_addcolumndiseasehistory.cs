namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumndiseasehistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Icd10Codes", "DiseaseHistory", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Icd10Codes", "DiseaseHistory");
        }
    }
}
