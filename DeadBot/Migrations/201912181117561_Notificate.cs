namespace DeadBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Notificate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DeadLines", "StartDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DeadLines", "StartDate", c => c.String());
        }
    }
}
