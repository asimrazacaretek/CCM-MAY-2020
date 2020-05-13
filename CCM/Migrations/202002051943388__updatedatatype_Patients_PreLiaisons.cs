namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _updatedatatype_Patients_PreLiaisons : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Patients_PreLiaisons", "CreatedOn", c => c.DateTime());
            AlterColumn("dbo.Patients_PreLiaisons", "UpdatedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Patients_PreLiaisons", "UpdatedOn", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Patients_PreLiaisons", "CreatedOn", c => c.DateTime(nullable: false));
        }
    }
}
