using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data.SqlClient;

namespace Common
{
    public class ConnectionTenantDb
    {
        // ConnectionString to Catalog
        private static SqlConnection catalogDbConnection = new SqlConnection("Server=tcp:exjobb-exempelapp.database.windows.net,1433;Initial Catalog=Catalog;Persist Security Info=False;User ID=Guest_CRM;Password=TreasuryGast!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30");

        public static string GetConnectionString()
        {
            string tenantId;
            string connectionString = null;

            // HttpContext.Current is null when a migration is run
            if (HttpContext.Current != null)
            {
                var httpRequst = HttpContext.Current.Request;
                Debug.WriteLine("Creating SchoolContext for request url {0}", httpRequst.RawUrl);
                tenantId = httpRequst.QueryString.Get("Id");
                connectionString = GetConnectionStringForTenant(tenantId);
            }
            else
            {
                // Running Update-Database or similar
                connectionString = "Server=tcp:exjobb-exempelapp.database.windows.net,1433;Initial Catalog=Exjobb2;Persist Security Info=False;User ID=Guest_CRM;Password=TreasuryGast!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            }
            
            return connectionString;
        }

        public static string GetConnectionStringForTenant(string tenantId)
        {
            string connectionStringTenant = null;

            try
            {
                catalogDbConnection.Open();
                SqlCommand command = new SqlCommand("SELECT Connection FROM TenantsMeta WHERE TenantID = " + tenantId.Trim() + ";", catalogDbConnection);
                connectionStringTenant = (string)command.ExecuteScalar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                catalogDbConnection.Close();
            }

            return connectionStringTenant;
        }
    }
}
