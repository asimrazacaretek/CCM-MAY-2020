namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Patients_PreLiaisonsId_Coloumn_AddedIn_patientsHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients_History", "Patients_PreLiaisonsId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients_History", "Patients_PreLiaisonsId");
        }
    }
}
