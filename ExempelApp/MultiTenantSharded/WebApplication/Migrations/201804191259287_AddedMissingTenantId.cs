namespace WebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMissingTenantId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CourseInstructor", "TenantID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CourseInstructor", "TenantID");
        }
    }
}
