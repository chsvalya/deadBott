namespace DeadBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTelegramIDTypeForUser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "TelegramId", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "TelegramId", c => c.Int(nullable: false));
        }
    }
}
