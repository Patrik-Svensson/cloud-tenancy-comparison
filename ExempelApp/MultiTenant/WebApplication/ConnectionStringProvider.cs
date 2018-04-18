using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication
{
    public class ConnectionStringProvider : ISettingsProvider
    {
        public static ConnectionStringProvider connectionstringProvider = new ConnectionStringProvider();

        public object Get(ITenantIdProvider idProvider)
        {
            return Common.ConnectionTenantDb.GetConnectionStringForTenant(idProvider.TenantId());
        }
    }
}