namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renamecolumnTypefrombytetoStringinLaboratoryResult : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Patient_LaboratoryResult", "Files", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Patient_LaboratoryResult", "Files", c => c.Binary());
        }
    }
}
