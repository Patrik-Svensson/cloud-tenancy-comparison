using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication
{
    public class CatalogSettingsProvider : ISettingsProvider
    {
        private readonly ITenantIdProvider idProvider;

        public CatalogSettingsProvider(ITenantIdProvider idProvider)
        {
            this.idProvider = idProvider;
        }

        public string GetConnectionString()
        {
            return Common.ConnectionTenantDb.GetConnectionStringForTenant(idProvider.TenantId());
        }

        public string GetDisplayName()
        {
            return Common.ConnectionTenantDb.GetDisplayNameForTenant(idProvider.TenantId());
        }
    }
}