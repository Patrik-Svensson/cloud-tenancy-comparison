using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication
{
    public class CatalogProvider : ISettingsProvider
    {
        public object Get(ITenantIdProvider idProvider)
        {
            return Common.ConnectionTenantDb.GetDisplayNameForTenant(idProvider.TenantId());
        }
    }
}