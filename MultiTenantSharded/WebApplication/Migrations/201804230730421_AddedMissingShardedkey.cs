namespace WebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMissingShardedkey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CourseInstructor", "TenantID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CourseInstructor", "TenantID");
        }
    }
}
