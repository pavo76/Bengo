namespace Bengo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KanaPractice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Kana_Practice",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserName = c.Int(nullable: false),
                        KanaID = c.Int(nullable: false),
                        LastPracticed = c.DateTime(nullable: false),
                        RepeatInterval = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Kanas", t => t.KanaID, cascadeDelete: true)
                .Index(t => t.KanaID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Kana_Practice", "KanaID", "dbo.Kanas");
            DropIndex("dbo.Kana_Practice", new[] { "KanaID" });
            DropTable("dbo.Kana_Practice");
        }
    }
}
