namespace Bengo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TextTitleAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Texts", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Texts", "Title");
        }
    }
}
