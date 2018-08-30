namespace Bengo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GoalMechanic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserDatas", "GoalID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserDatas", "GoalID");
        }
    }
}
