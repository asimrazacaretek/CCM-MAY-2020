namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnsPatientProfile_UrgencyContact_AddNewTablesPatientProfile_ProfesionalCareProviderANDPatientProfile_DieseaseState : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PatientProfile_DieseaseState",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DieseaseStateType = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PatientProfile_ProfesionalCareProvider",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CareProvider = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.PatientProfile_UrgencyContact", "PrimaryProfesionalCareProvider", c => c.String());
            AddColumn("dbo.PatientProfile_UrgencyContact", "PrimaryExpertise", c => c.String());
            AddColumn("dbo.PatientProfile_UrgencyContact", "PrimaryHealthProxyAndCarplane", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientProfile_UrgencyContact", "PrimaryDieseaseState", c => c.String());
            AddColumn("dbo.PatientProfile_UrgencyContact", "SecondaryProfesionalCareProvider", c => c.String());
            AddColumn("dbo.PatientProfile_UrgencyContact", "SecondaryExpertise", c => c.String());
            AddColumn("dbo.PatientProfile_UrgencyContact", "SecondaryHealthProxyAndCarplane", c => c.Boolean(nullable: false));
            AddColumn("dbo.PatientProfile_UrgencyContact", "SecondaryDieseaseState", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientProfile_UrgencyContact", "SecondaryDieseaseState");
            DropColumn("dbo.PatientProfile_UrgencyContact", "SecondaryHealthProxyAndCarplane");
            DropColumn("dbo.PatientProfile_UrgencyContact", "SecondaryExpertise");
            DropColumn("dbo.PatientProfile_UrgencyContact", "SecondaryProfesionalCareProvider");
            DropColumn("dbo.PatientProfile_UrgencyContact", "PrimaryDieseaseState");
            DropColumn("dbo.PatientProfile_UrgencyContact", "PrimaryHealthProxyAndCarplane");
            DropColumn("dbo.PatientProfile_UrgencyContact", "PrimaryExpertise");
            DropColumn("dbo.PatientProfile_UrgencyContact", "PrimaryProfesionalCareProvider");
            DropTable("dbo.PatientProfile_ProfesionalCareProvider");
            DropTable("dbo.PatientProfile_DieseaseState");
        }
    }
}
