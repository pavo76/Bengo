namespace Bengo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditedTextModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Texts", "Kana_List", c => c.String());
            AddColumn("dbo.Texts", "ForLearningKana", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Texts", "ForLearningKana");
            DropColumn("dbo.Texts", "Kana_List");
        }
    }
}
