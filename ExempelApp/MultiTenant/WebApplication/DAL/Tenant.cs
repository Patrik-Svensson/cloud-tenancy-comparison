using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace WebApplication.DAL
{
    public class Tenant
    {
        // TODO: Fixa hårdkodning
        private static Dictionary<string, Tenant> Tenants = new Dictionary<string, Tenant>()
        {
            { "1", new Tenant( "1", Common.ConnectionTenantDb.GetConnectionStringForTenant("1")) },
            { "2", new Tenant( "2", Common.ConnectionTenantDb.GetConnectionStringForTenant("2")) }
        };

        public Tenant(string id, string connectionString)
        {
            this.id = id;
            this.connectionString = connectionString;
        }

        public string id { get; }
        public string connectionString { get; }

        public static Tenant getTenant(string id)
        {
            if (Tenants.ContainsKey(id))
                return Tenants[id];

            return null;
        }

        // TODO: Add Settings for specific tenant
    }
}