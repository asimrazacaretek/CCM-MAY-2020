namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changelisaonassingedontype : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Patients", "LiasionAssignedOn", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Patients", "LiasionAssignedOn", c => c.DateTime(nullable: false));
        }
    }
}
