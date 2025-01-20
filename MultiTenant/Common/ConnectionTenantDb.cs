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
        private static readonly bool isCaching = true;
        private static readonly MemoryCache cache = new MemoryCache("ConnectionString");
        private static readonly MemoryCache cache_2 = new MemoryCache("Displayname");
        public static string GetConnectionString()
        {
            string tenantId;
            string connectionString = null;

            // HttpContext.Current is null when running migrations
            if (HttpContext.Current != null)
            {
                var httpRequst = HttpContext.Current.Request;
                Debug.WriteLine("Creating SchoolContext for request url {0}", httpRequst.RawUrl);
                tenantId = httpRequst.QueryString.Get("TenantId");

                //@startup eg. localhost:53369/ no tenant id is given. For testing purposes.
                if (tenantId == null)
                {
                    tenantId = "1";
                   
                }
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
            string result;

            SqlConnection catalogDbConnection = new SqlConnection("Server=tcp:exjobb-exempelapp.database.windows.net,1433;Initial Catalog=CatalogMulti;Persist Security Info=False;User ID=Guest_CRM;Password=TreasuryGast!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            if (isCaching)
            {
                result = (string)cache.Get(tenantId);
                if (result != null)
                    return result;
            }

            try
            {
                catalogDbConnection.Open();
                SqlCommand command = new SqlCommand("SELECT Connection FROM TenantsMeta WHERE TenantID = " + tenantId.Trim() + ";", catalogDbConnection);
                connectionStringTenant = (string)command.ExecuteScalar();
                if (isCaching)
                {
                    if (connectionStringTenant != null)
                        cache.Set(tenantId, connectionStringTenant, DateTimeOffset.Now.AddMinutes(1));
                }
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
            SqlConnection catalogDbConnection = new SqlConnection("Server=tcp:exjobb-exempelapp.database.windows.net,1433;Initial Catalog=CatalogMulti;Persist Security Info=False;User ID=Guest_CRM;Password=TreasuryGast!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            string displayName = null;
            if (isCaching)
            {
                string result = (string)cache_2.Get(tenantId);
                if (result != null)
                    return result;
            }

            try
            {
                catalogDbConnection.Open();
                SqlCommand command = new SqlCommand("SELECT DisplayName FROM TenantsMeta WHERE TenantID = " + tenantId.Trim() + ";", catalogDbConnection);
                displayName = (string)command.ExecuteScalar();
                if (isCaching)
                {
                    if (displayName != null)
                        cache_2.Set(tenantId, displayName, DateTimeOffset.Now.AddMinutes(1));
                }
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
