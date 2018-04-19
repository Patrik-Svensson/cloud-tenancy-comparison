namespace WebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTenantId1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Person", "TenantID", c => c.Int(nullable: false));
            DropColumn("dbo.Person", "TenantId1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Person", "TenantId1", c => c.Int());
            AlterColumn("dbo.Person", "TenantID", c => c.Int());
        }
    }
}
