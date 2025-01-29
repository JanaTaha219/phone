using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Data;
using WebApplication6.Models;

namespace WebApplication6.Pages.Analytics
{
    public class AnalyticsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Employee> TopSearchedUsers { get; set; } = new();
        public List<string> Departments { get; set; } = new(); // Store all unique departments

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Department { get; set; }

        public async Task OnGetAsync()
        {
            // Fetch all departments from the database
            Departments = await _context.Employees
                .Where(e => !string.IsNullOrEmpty(e.Department))
                .Select(e => e.Department!)
                .Distinct()
                .ToListAsync();

            var query = _context.Employees.AsQueryable();

            // Apply department filter
            if (!string.IsNullOrEmpty(Department))
            {
                query = query.Where(e => e.Department == Department);
            }

            // Console.WriteLine(query);

            // Apply date range filter
            if (StartDate.HasValue && EndDate.HasValue)
            {
                query = query.Where(e => e.SearchHistories.Any(sh => sh.SearchDate >= StartDate.Value && sh.SearchDate <= EndDate.Value));
            }


            TopSearchedUsers = await query
                .OrderByDescending(e => e.counter)
                .Take(10)
                .ToListAsync();
        }

    }


}
