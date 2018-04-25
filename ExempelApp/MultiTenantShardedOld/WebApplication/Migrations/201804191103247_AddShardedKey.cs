namespace WebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShardedKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Course", "ShardedID", c => c.Int(nullable: false));
            AddColumn("dbo.Department", "ShardedID", c => c.Int(nullable: false));
            AddColumn("dbo.Person", "ShardedID", c => c.Int(nullable: false));
            AddColumn("dbo.OfficeAssignment", "ShardedID", c => c.Int(nullable: false));
            AddColumn("dbo.Enrollment", "ShardedID", c => c.Int(nullable: false));
            AlterStoredProcedure(
                "dbo.Department_Insert",
                p => new
                    {
                        ShardedID = p.Int(),
                        Name = p.String(maxLength: 50),
                        Budget = p.Decimal(precision: 19, scale: 4, storeType: "money"),
                        StartDate = p.DateTime(),
                        InstructorID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Department]([ShardedID], [Name], [Budget], [StartDate], [InstructorID])
                      VALUES (@ShardedID, @Name, @Budget, @StartDate, @InstructorID)
                      
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
                        ShardedID = p.Int(),
                        Name = p.String(maxLength: 50),
                        Budget = p.Decimal(precision: 19, scale: 4, storeType: "money"),
                        StartDate = p.DateTime(),
                        InstructorID = p.Int(),
                        RowVersion_Original = p.Binary(maxLength: 8, fixedLength: true, storeType: "rowversion"),
                    },
                body:
                    @"UPDATE [dbo].[Department]
                      SET [ShardedID] = @ShardedID, [Name] = @Name, [Budget] = @Budget, [StartDate] = @StartDate, [InstructorID] = @InstructorID
                      WHERE (([DepartmentID] = @DepartmentID) AND (([RowVersion] = @RowVersion_Original) OR ([RowVersion] IS NULL AND @RowVersion_Original IS NULL)))
                      
                      SELECT t0.[RowVersion]
                      FROM [dbo].[Department] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[DepartmentID] = @DepartmentID"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enrollment", "ShardedID");
            DropColumn("dbo.OfficeAssignment", "ShardedID");
            DropColumn("dbo.Person", "ShardedID");
            DropColumn("dbo.Department", "ShardedID");
            DropColumn("dbo.Course", "ShardedID");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
