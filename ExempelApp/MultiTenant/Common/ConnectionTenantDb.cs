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
            SqlConnection catalogDbConnection = new SqlConnection("Server=tcp:exjobb-exempelapp.database.windows.net,1433;Initial Catalog=CatalogMulti;Persist Security Info=False;User ID=Guest_CRM;Password=TreasuryGast!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

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

         //   if (connectionStringTenant == null)
         //       connectionStringTenant = "Server=tcp:exjobb-exempelapp.database.windows.net,1433;Initial Catalog=ExjobbMulti01;Persist Security Info=False;User ID=Guest_CRM;Password=TreasuryGast!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            return connectionStringTenant;
        }

        public static string GetDisplayNameForTenant(string tenantId)
        {
            SqlConnection catalogDbConnection = new SqlConnection("Server=tcp:exjobb-exempelapp.database.windows.net,1433;Initial Catalog=CatalogMulti;Persist Security Info=False;User ID=Guest_CRM;Password=TreasuryGast!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

            string displayName = null;

            try
            {
                catalogDbConnection.Open();
                SqlCommand command = new SqlCommand("SELECT DisplayName FROM TenantsMeta WHERE TenantID = " + tenantId.Trim() + ";", catalogDbConnection);
                displayName = (string)command.ExecuteScalar(); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                catalogDbConnection.Close();
            }
         //   if (displayName == null)
         //       displayName = "Freddys Magväskor";

            return displayName;  
        }
    }
}
