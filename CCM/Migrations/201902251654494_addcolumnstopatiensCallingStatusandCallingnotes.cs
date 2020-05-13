namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnstopatiensCallingStatusandCallingnotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Patients", "CallingStatus", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.Patients", "CallingNote", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Patients", "CallingNote");
            DropColumn("dbo.Patients", "CallingStatus");
        }
    }
}
