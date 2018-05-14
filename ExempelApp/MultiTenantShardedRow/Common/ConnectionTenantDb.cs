using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data.SqlClient;
using System.Runtime.Caching;

namespace Common
{
    public class ConnectionTenantDb
    {

        private static readonly MemoryCache cache = new MemoryCache("ConnectionString");
        private static readonly MemoryCache cache_2 = new MemoryCache("Displayname");

        public static string GetConnectionStringForTenant(string tenantId)
        {
            string connectionStringTenant = null;
            SqlConnection catalogDbConnection = new SqlConnection("Server=tcp:exjobb-exempelapp.database.windows.net,1433;Initial Catalog=CatalogSharded;Persist Security Info=False;User ID=Guest_CRM;Password=TreasuryGast!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30");

            string result = (string)cache.Get(tenantId);
            if (result != null)
                return result;



            try
            {
                catalogDbConnection.Open();
                SqlCommand command = new SqlCommand("SELECT Connection FROM TenantsMeta WHERE TenantID = " + tenantId.Trim() + ";", catalogDbConnection);
                connectionStringTenant = (string)command.ExecuteScalar();

                if (connectionStringTenant != null)
                    cache.Set(tenantId, connectionStringTenant, DateTimeOffset.Now.AddMinutes(1));

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

        public static string GetDisplayNameForTenant(string tenantId)
        {

            string displayName = null;
            SqlConnection catalogDbConnection = new SqlConnection("Server=tcp:exjobb-exempelapp.database.windows.net,1433;Initial Catalog=CatalogSharded;Persist Security Info=False;User ID=Guest_CRM;Password=TreasuryGast!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30");
            string result = (string)cache_2.Get(tenantId);
            if (result != null)
                return result;


            try
            {
                catalogDbConnection.Open();
                SqlCommand command = new SqlCommand("SELECT DisplayName FROM TenantsMeta WHERE TenantID = " + tenantId.Trim() + ";", catalogDbConnection);
                displayName = (string)command.ExecuteScalar();

                if (displayName != null)
                    cache_2.Set(tenantId, displayName, DateTimeOffset.Now.AddMinutes(1));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                catalogDbConnection.Close();
            }

            return displayName;  
        }
    }
}
