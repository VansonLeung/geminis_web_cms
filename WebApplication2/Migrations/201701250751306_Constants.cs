namespace WebApplication2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Constants : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "pageShouldShowTopbarmenu", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "pageShouldShowTopbarmenu");
        }
    }
}
