namespace DeadBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DeadLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DateTime = c.DateTime(nullable: false),
                        Priority = c.Int(nullable: false),
                        NotificationFrequency = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        ChatId = c.Long(nullable: false),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TelegramId = c.Int(nullable: false),
                        IsBot = c.Boolean(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Username = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DeadLines", "User_Id", "dbo.Users");
            DropIndex("dbo.DeadLines", new[] { "User_Id" });
            DropTable("dbo.Users");
            DropTable("dbo.DeadLines");
        }
    }
}
