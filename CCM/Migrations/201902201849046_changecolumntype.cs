namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumntype : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.EnrollmentSubstatusReasons");
            AlterColumn("dbo.EnrollmentSubstatusReasons", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.EnrollmentSubstatusReasons", "Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.EnrollmentSubstatusReasons");
            AlterColumn("dbo.EnrollmentSubstatusReasons", "Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.EnrollmentSubstatusReasons", "Id");
        }
    }
}
