using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Serilog;

namespace MigrateDatabase
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void Migrate(string tenantId)
        {

            var configuration = new WebApplication.Migrations.Configuration
            {
                TargetDatabase = new System.Data.Entity.Infrastructure.DbConnectionInfo(Common.ConnectionTenantDb.GetConnectionStringForTenant(tenantId), "System.Data.SqlClient"),
                MigrationsDirectory = "Migrations",
            };


            MigratorBase migrator = new DbMigrator(configuration);
            migrator = new MigratorLoggingDecorator(migrator, new MyLogger());

            var dbMigrations = migrator.GetDatabaseMigrations();
            var m = migrator.GetLocalMigrations();

            // Run migrations (and possibly seed)
            migrator.Update();
        }
    }

    public class MyLogger : System.Data.Entity.Migrations.Infrastructure.MigrationsLogger
    {
        public override void Info(string message)
        {
            // Short status messages come here
            Log.Information(message);
        }

        public override void Verbose(string message)
        {
            // The SQL text and other info comes here
            Log.Verbose(message);
        }

        public override void Warning(string message)
        {
            // Warnings and other bad messages come here
            Log.Warning(message);
        }
    }
}
