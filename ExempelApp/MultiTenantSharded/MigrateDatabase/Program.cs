using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Serilog;

namespace MigrateDatabase
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static int Main(string[] arguments)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Verbose)
                .CreateLogger();

            string tenant;

            if (arguments.Length != 1)
            {
                Console.WriteLine("TenantId: ");
                tenant = Console.ReadLine().Trim();
            }
            else
            {
                tenant = arguments[0];
            }

            Functions.Migrate(tenant);


            // The following code ensures that the WebJob will be running continuously
            // host.Call(typeof(Functions).GetMethod(nameof(Functions.Migrate)));
            
            return 0;
        }
    }
}
