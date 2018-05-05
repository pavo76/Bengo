namespace Bengo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Practice_Tables_Fix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Grammar_Practice", "UserData_ID", "dbo.UserDatas");
            DropForeignKey("dbo.Kanji_Practice", "UserData_ID", "dbo.UserDatas");
            DropForeignKey("dbo.Vocabulary_Practice", "UserData_ID", "dbo.UserDatas");
            DropIndex("dbo.Grammar_Practice", new[] { "UserData_ID" });
            DropIndex("dbo.Kanji_Practice", new[] { "UserData_ID" });
            DropIndex("dbo.Vocabulary_Practice", new[] { "UserData_ID" });
            AddColumn("dbo.Grammar_Practice", "UserName", c => c.Int(nullable: false));
            AddColumn("dbo.Kanji_Practice", "UserName", c => c.String());
            AddColumn("dbo.Vocabulary_Practice", "UserName", c => c.String());
            DropColumn("dbo.Grammar_Practice", "UserID");
            DropColumn("dbo.Grammar_Practice", "UserData_ID");
            DropColumn("dbo.Kanji_Practice", "UserID");
            DropColumn("dbo.Kanji_Practice", "UserData_ID");
            DropColumn("dbo.Vocabulary_Practice", "UserID");
            DropColumn("dbo.Vocabulary_Practice", "UserData_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Vocabulary_Practice", "UserData_ID", c => c.Int());
            AddColumn("dbo.Vocabulary_Practice", "UserID", c => c.Int(nullable: false));
            AddColumn("dbo.Kanji_Practice", "UserData_ID", c => c.Int());
            AddColumn("dbo.Kanji_Practice", "UserID", c => c.Int(nullable: false));
            AddColumn("dbo.Grammar_Practice", "UserData_ID", c => c.Int());
            AddColumn("dbo.Grammar_Practice", "UserID", c => c.Int(nullable: false));
            DropColumn("dbo.Vocabulary_Practice", "UserName");
            DropColumn("dbo.Kanji_Practice", "UserName");
            DropColumn("dbo.Grammar_Practice", "UserName");
            CreateIndex("dbo.Vocabulary_Practice", "UserData_ID");
            CreateIndex("dbo.Kanji_Practice", "UserData_ID");
            CreateIndex("dbo.Grammar_Practice", "UserData_ID");
            AddForeignKey("dbo.Vocabulary_Practice", "UserData_ID", "dbo.UserDatas", "ID");
            AddForeignKey("dbo.Kanji_Practice", "UserData_ID", "dbo.UserDatas", "ID");
            AddForeignKey("dbo.Grammar_Practice", "UserData_ID", "dbo.UserDatas", "ID");
        }
    }
}
