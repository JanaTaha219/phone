using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Data;
using WebApplication6.Models;

namespace WebApplication6.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _applicationDbContext;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        // Search handler
        public async Task<IActionResult> OnGetSearchAsync([FromQuery] string query, [FromQuery] string department)
        {
            if (string.IsNullOrWhiteSpace(query) && string.IsNullOrWhiteSpace(department))
            {
                return BadRequest("Query and department cannot be both empty.");
            }

            var queryable = _applicationDbContext.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                queryable = queryable.Where(n => EF.Functions.Like(n.Name, $"{query}%"));
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                queryable = queryable.Where(d => d.Department == department);
            }

            var results = await queryable
                .Select(n => new { n.Name, n.Id })  // Return ID for further fetching
                .Take(10)
                .ToListAsync();

            return new JsonResult(results);
        }


        // Employee details handler
        public async Task<IActionResult> OnGetEmployeeDetailsAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid employee ID.");
            }

            try
            {
                var employee = await _applicationDbContext.Employees.FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    return NotFound("Employee not found.");
                }

                _applicationDbContext.Employees.Attach(employee);
                employee.counter++;
                await _applicationDbContext.SaveChangesAsync();

                var employeeDetails = new
                {
                    employee.Name,
                    employee.Email,
                    employee.LocalPhone,
                    employee.Department,
                    employee.Position,
                    employee.OfficeLocation,
                    employee.Notes,
                    employee.PhoneNumber,
                    employee.OtherNumbers
                };

                return new JsonResult(employeeDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employee details.");
                return new JsonResult(new Employee());
            }
        }



        public void OnGet()
        {
        }
    }
}
