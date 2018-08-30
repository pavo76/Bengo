namespace Bengo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Kana_Practice_Correct_UserID : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Kana_Practice", "UserName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Kana_Practice", "UserName", c => c.Int(nullable: false));
        }
    }
}
