namespace Bengo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserRelatedData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Grammar_Practice",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        GrammarID = c.Int(nullable: false),
                        LastPracticed = c.DateTime(nullable: false),
                        RepeatInterval = c.Int(nullable: false),
                        UserData_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Grammars", t => t.GrammarID, cascadeDelete: true)
                .ForeignKey("dbo.UserDatas", t => t.UserData_ID)
                .Index(t => t.GrammarID)
                .Index(t => t.UserData_ID);
            
            CreateTable(
                "dbo.UserDatas",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserName = c.Int(nullable: false),
                        VocabularyList = c.String(),
                        KanjiList = c.String(),
                        GrammarList = c.String(),
                        KanaList = c.String(),
                        KanaDone = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Kanas",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Letter = c.String(),
                        Reading = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Kanji_Practice",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        KanjiID = c.Int(nullable: false),
                        LastPracticed = c.DateTime(nullable: false),
                        RepeatInterval = c.Int(nullable: false),
                        UserData_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Kanjis", t => t.KanjiID, cascadeDelete: true)
                .ForeignKey("dbo.UserDatas", t => t.UserData_ID)
                .Index(t => t.KanjiID)
                .Index(t => t.UserData_ID);
            
            CreateTable(
                "dbo.RepeatIntervals",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Interval = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Vocabulary_Practice",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        VocabularyID = c.Int(nullable: false),
                        LastPracticed = c.DateTime(nullable: false),
                        RepeatInterval = c.Int(nullable: false),
                        UserData_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserDatas", t => t.UserData_ID)
                .ForeignKey("dbo.Vocabularies", t => t.VocabularyID, cascadeDelete: true)
                .Index(t => t.VocabularyID)
                .Index(t => t.UserData_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Vocabulary_Practice", "VocabularyID", "dbo.Vocabularies");
            DropForeignKey("dbo.Vocabulary_Practice", "UserData_ID", "dbo.UserDatas");
            DropForeignKey("dbo.Kanji_Practice", "UserData_ID", "dbo.UserDatas");
            DropForeignKey("dbo.Kanji_Practice", "KanjiID", "dbo.Kanjis");
            DropForeignKey("dbo.Grammar_Practice", "UserData_ID", "dbo.UserDatas");
            DropForeignKey("dbo.Grammar_Practice", "GrammarID", "dbo.Grammars");
            DropIndex("dbo.Vocabulary_Practice", new[] { "UserData_ID" });
            DropIndex("dbo.Vocabulary_Practice", new[] { "VocabularyID" });
            DropIndex("dbo.Kanji_Practice", new[] { "UserData_ID" });
            DropIndex("dbo.Kanji_Practice", new[] { "KanjiID" });
            DropIndex("dbo.Grammar_Practice", new[] { "UserData_ID" });
            DropIndex("dbo.Grammar_Practice", new[] { "GrammarID" });
            DropTable("dbo.Vocabulary_Practice");
            DropTable("dbo.RepeatIntervals");
            DropTable("dbo.Kanji_Practice");
            DropTable("dbo.Kanas");
            DropTable("dbo.UserDatas");
            DropTable("dbo.Grammar_Practice");
        }
    }
}
