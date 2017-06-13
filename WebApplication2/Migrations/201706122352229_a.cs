namespace WebApplication2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class a : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Constants", "Desc", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Constants", "Desc");
        }
    }
}
