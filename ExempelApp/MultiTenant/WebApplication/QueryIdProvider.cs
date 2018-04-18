using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication
{
    public class QueryIdProvider : ITenantIdProvider
    {
        private string _tenantId;

        public QueryIdProvider()
        {
            _tenantId = HttpContext.Current.Request.QueryString.Get("TenantId");
        }

        public string TenantId()
        {
            return _tenantId;
        }
    }
}