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
        }

        public string TenantId()
        {
            _tenantId = HttpContext.Current.Request.QueryString.Get("TenantId");

            //FOR TESTING
            if(_tenantId == null)
            {
                return "1";
            }
          
            return _tenantId;
        }
    }
}