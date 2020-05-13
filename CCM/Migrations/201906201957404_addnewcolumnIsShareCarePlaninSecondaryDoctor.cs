namespace CCM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnewcolumnIsShareCarePlaninSecondaryDoctor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SecondaryDoctors", "IsShareCarePlan", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SecondaryDoctors", "IsShareCarePlan");
        }
    }
}
