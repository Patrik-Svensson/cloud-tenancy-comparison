namespace WebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedPropertyName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Course", "TenantID", c => c.Int(nullable: false));
            AddColumn("dbo.Department", "TenantID", c => c.Int(nullable: false));
            AddColumn("dbo.Person", "TenantID", c => c.Int(nullable: false));
            AddColumn("dbo.OfficeAssignment", "TenantID", c => c.Int(nullable: false));
            AddColumn("dbo.Enrollment", "TenantID", c => c.Int(nullable: false));
            DropColumn("dbo.Course", "ShardedID");
            DropColumn("dbo.Department", "ShardedID");
            DropColumn("dbo.Person", "ShardedID");
            DropColumn("dbo.OfficeAssignment", "ShardedID");
            DropColumn("dbo.Enrollment", "ShardedID");
            AlterStoredProcedure(
                "dbo.Department_Insert",
                p => new
                    {
                        TenantID = p.Int(),
                        Name = p.String(maxLength: 50),
                        Budget = p.Decimal(precision: 19, scale: 4, storeType: "money"),
                        StartDate = p.DateTime(),
                        InstructorID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Department]([TenantID], [Name], [Budget], [StartDate], [InstructorID])
                      VALUES (@TenantID, @Name, @Budget, @StartDate, @InstructorID)
                      
                      DECLARE @DepartmentID int
                      SELECT @DepartmentID = [DepartmentID]
                      FROM [dbo].[Department]
                      WHERE @@ROWCOUNT > 0 AND [DepartmentID] = scope_identity()
                      
                      SELECT t0.[DepartmentID], t0.[RowVersion]
                      FROM [dbo].[Department] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[DepartmentID] = @DepartmentID"
            );
            
            AlterStoredProcedure(
                "dbo.Department_Update",
                p => new
                    {
                        DepartmentID = p.Int(),
                        TenantID = p.Int(),
                        Name = p.String(maxLength: 50),
                        Budget = p.Decimal(precision: 19, scale: 4, storeType: "money"),
                        StartDate = p.DateTime(),
                        InstructorID = p.Int(),
                        RowVersion_Original = p.Binary(maxLength: 8, fixedLength: true, storeType: "rowversion"),
                    },
                body:
                    @"UPDATE [dbo].[Department]
                      SET [TenantID] = @TenantID, [Name] = @Name, [Budget] = @Budget, [StartDate] = @StartDate, [InstructorID] = @InstructorID
                      WHERE (([DepartmentID] = @DepartmentID) AND (([RowVersion] = @RowVersion_Original) OR ([RowVersion] IS NULL AND @RowVersion_Original IS NULL)))
                      
                      SELECT t0.[RowVersion]
                      FROM [dbo].[Department] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[DepartmentID] = @DepartmentID"
            );
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.Enrollment", "ShardedID", c => c.Int(nullable: false));
            AddColumn("dbo.OfficeAssignment", "ShardedID", c => c.Int(nullable: false));
            AddColumn("dbo.Person", "ShardedID", c => c.Int(nullable: false));
            AddColumn("dbo.Department", "ShardedID", c => c.Int(nullable: false));
            AddColumn("dbo.Course", "ShardedID", c => c.Int(nullable: false));
            DropColumn("dbo.Enrollment", "TenantID");
            DropColumn("dbo.OfficeAssignment", "TenantID");
            DropColumn("dbo.Person", "TenantID");
            DropColumn("dbo.Department", "TenantID");
            DropColumn("dbo.Course", "TenantID");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
