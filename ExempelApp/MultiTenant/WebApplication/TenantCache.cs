using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using WebApplication.Models;
using WebApplication.DAL;

public class TenantCache : ICache
{
    private MemoryCache cache;
    private SchoolContext db;
    string tenantId;

    public TenantCache(string tenantId)
    {
        this.tenantId = tenantId;
        cache = new MemoryCache(tenantId);
    }

    public IEnumerable<Course> GetCachedData(int selectedDepartment, int departmentId, string tenantId)
    {
        string cacheKey = $"CourseController.LoadCourses({selectedDepartment},{departmentId})";

        List<Course> result = (List<Course>)cache.Get(cacheKey);

        if (result == null)
        {
            result = LoadCoursesFromDatabase(SelectedDepartment, departmentID);

            // Cache for 
            cache.Add(cacheKey, result, DateTimeOffset.Now.AddMinutes(1));
        }

        return null;
    }

    private List<Course> LoadCoursesFromDatabase(int? SelectedDepartment, int departmentID)
    {
        return db.Courses
            .Where(c => !SelectedDepartment.HasValue || c.DepartmentID == departmentID)
            .OrderBy(d => d.CourseID)
            .Include(d => d.Department)
            .ToList();
    }
}
