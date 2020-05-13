namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnisshowninpopup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PatientNotes", "isToShowinPopup", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PatientNotes", "isToShowinPopup");
        }
    }
}
