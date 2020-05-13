namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changename : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Patient_History", newName: "Patients_History");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Patients_History", newName: "Patient_History");
        }
    }
}
