using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Web;

namespace WebApplication
{

    public class CourseInterceptor : IDbCommandInterceptor
    {
        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
          //  command.CommandText = command.CommandText.ToString() + " WHERE TenantID=" + HttpContext.Current.Request.QueryString.Get("TenantId");
                     throw new NotImplementedException(command.CommandText.ToString());
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
          //  command.CommandText = command.CommandText.ToString() + " WHERE TenantID=" + HttpContext.Current.Request.QueryString.Get("TenantId");
               throw new NotImplementedException(command.CommandText.ToString());
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            //   command.CommandText = command.CommandText.ToString() + " WHERE TenantID=" + HttpContext.Current.Request.QueryString.Get("TenantId");
            //  SetTenantParameterValue(command);
            throw new NotImplementedException(command.CommandText.ToString());
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            //   command.CommandText = command.CommandText.ToString() + " WHERE TenantID=" + HttpContext.Current.Request.QueryString.Get("TenantId");
            throw new NotImplementedException(command.CommandText.ToString());
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
         //   command.CommandText = command.CommandText.ToString() + " WHERE TenantID=" + HttpContext.Current.Request.QueryString.Get("TenantId");
              throw new NotImplementedException(command.CommandText.ToString());
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            //   command.CommandText = command.CommandText.ToString() + " WHERE TenantID=" + HttpContext.Current.Request.QueryString.Get("TenantId");
            throw new NotImplementedException(command.CommandText.ToString());
        }

        private static void SetTenantParameterValue(DbCommand command)
        {
            if (command == null || command.Parameters.Count == 0)
            {
                return;
            }

            foreach (DbParameter param in command.Parameters)
            {
                throw new Exception("Habibi");
            }
            //var identity = Thread.CurrentPrincipal.Identity as ClaimsIdentity;
            //  if ((command == null) || (command.Parameters.Count == 0))// || identity == null)
            /*{
                return;
            }
            var userClaim = identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userClaim != null)
            {
                var userId = userClaim.Value;
                // Enumerate all command parameters and assign the correct value in the one we added inside query visitor
                foreach (DbParameter param in command.Parameters)
                {
                    if (param.ParameterName != TenantAwareAttribute.TenantIdFilterParameterName)
                        return;
                    param.Value = userId;
                }
            }*/
        }
    }
}