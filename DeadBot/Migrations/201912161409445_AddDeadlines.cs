namespace DeadBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeadlines : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.DeadLines", "Priority");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DeadLines", "Priority", c => c.Int(nullable: false));
        }
    }
}
