using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace MigrateDatabase
{
	public class Functions
	{
		// This function will get triggered/executed when a new message is written 
		// on an Azure Queue called queue.
		public static void Migrate(TextWriter log)
		{
            var configuration = new DbMigrationsConfiguration<WebApplication.DAL.SchoolContext>();
            var migrator = new DbMigrator(configuration);

            migrator.Update();
		}
	}
}
