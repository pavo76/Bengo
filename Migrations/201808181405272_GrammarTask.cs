namespace Bengo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrammarTask : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GrammarTasks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GrammarID = c.Int(nullable: false),
                        Question = c.String(),
                        CorrectAnswer = c.String(),
                        WrongAnswer1 = c.String(),
                        WrongAnswer2 = c.String(),
                        WrongAnswer3 = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Grammars", t => t.GrammarID, cascadeDelete: true)
                .Index(t => t.GrammarID);
            
            DropTable("dbo.GrammarQuestions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.GrammarQuestions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GrammarID = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Question = c.String(),
                        Answer1 = c.String(),
                        Answer2 = c.String(),
                        Answer3 = c.String(),
                        Answer4 = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            DropForeignKey("dbo.GrammarTasks", "GrammarID", "dbo.Grammars");
            DropIndex("dbo.GrammarTasks", new[] { "GrammarID" });
            DropTable("dbo.GrammarTasks");
        }
    }
}
