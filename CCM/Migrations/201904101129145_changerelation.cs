namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changerelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DoctorSchedules", "ClinicID", "dbo.Clinics");
            DropIndex("dbo.DoctorSchedules", new[] { "ClinicID" });
            DropTable("dbo.Clinics");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Clinics",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        City = c.String(),
                        StateCode = c.String(),
                        Zip = c.String(),
                        Phone = c.String(),
                        Web = c.String(),
                        Email = c.String(),
                        Fax = c.String(),
                        Photo = c.String(),
                        NPI = c.String(),
                        IntegraTaxID = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        PrimaryContactName = c.String(),
                        PrimaryContactNumber = c.String(),
                        SecondaryContactName = c.String(),
                        SecondaryContactNumber = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateIndex("dbo.DoctorSchedules", "ClinicID");
            AddForeignKey("dbo.DoctorSchedules", "ClinicID", "dbo.Clinics", "ID", cascadeDelete: true);
        }
    }
}
