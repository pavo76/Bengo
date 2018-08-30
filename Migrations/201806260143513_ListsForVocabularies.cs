namespace Bengo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ListsForVocabularies : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vocabularies", "KanjiList", c => c.String());
            AddColumn("dbo.Vocabularies", "KanaList", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vocabularies", "KanaList");
            DropColumn("dbo.Vocabularies", "KanjiList");
        }
    }
}
