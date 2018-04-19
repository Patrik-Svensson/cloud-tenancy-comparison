namespace WebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTenantId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Person", "TenantId1", c => c.Int());
            AlterColumn("dbo.Person", "TenantId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Person", "TenantId", c => c.Int(nullable: false));
            DropColumn("dbo.Person", "TenantId1");
        }
    }
}
