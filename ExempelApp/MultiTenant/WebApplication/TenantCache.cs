using System;
using System.Collections.Generic;
using WebApplication.Models;

public class TenantCache : ICache
{
    public static IEnumerable<Course> GetCachedData(int selectedDepartment, int departmentId, string tenantId)
    {
        return null;
    }
}
