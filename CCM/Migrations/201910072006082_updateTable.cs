namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Patient_LaboratoryResult", "Files", c => c.String(maxLength: 8000, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Patient_LaboratoryResult", "Files", c => c.String());
        }
    }
}
