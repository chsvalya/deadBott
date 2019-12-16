namespace DeadBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewUser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DeadLines", "DateTime", c => c.DateTime());
            AlterColumn("dbo.DeadLines", "StartDate", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DeadLines", "StartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DeadLines", "DateTime", c => c.DateTime(nullable: false));
        }
    }
}
