namespace Bengo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserDataFix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserDatas", "UserName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserDatas", "UserName", c => c.Int(nullable: false));
        }
    }
}
