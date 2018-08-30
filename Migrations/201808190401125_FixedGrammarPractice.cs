namespace Bengo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixedGrammarPractice : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Grammar_Practice", "UserName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Grammar_Practice", "UserName", c => c.Int(nullable: false));
        }
    }
}
