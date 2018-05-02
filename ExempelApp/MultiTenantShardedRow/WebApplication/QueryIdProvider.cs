using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication
{
    public class QueryIdProvider : ITenantIdProvider
    {
        private static string _tenantId;

        public QueryIdProvider()
        {   
        }

        public string TenantId()
        {
            _tenantId = HttpContext.Current.Request.QueryString.Get("TenantId");

            if(_tenantId == null)
            {
            
                _tenantId = "1";
                return _tenantId;
            }


            return _tenantId;
        }
    }
}