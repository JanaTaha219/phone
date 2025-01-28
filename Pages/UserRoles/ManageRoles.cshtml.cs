using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication6.Models;

namespace WebApplication6.Pages.UserRoles
{
    [Authorize(Roles = "Admin")]
    public class ManageRolesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ManageRolesModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [BindProperty]
        public string UserId { get; set; }
        [BindProperty]
        public string UserName { get; set; }
        [BindProperty]
        public List<RoleInfo> Roles { get; set; }
        public async Task<IActionResult> OnGetAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();
            UserId = user.Id;
            UserName = user.UserName;
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();
            Roles = allRoles.Select(role => new RoleInfo
            {
                RoleId = role.Id,
                RoleName = role.Name,
                IsSelected = userRoles.Contains(role.Name)
            }).ToList();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
                return NotFound();
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in Roles)
            {
                if (userRoles.Contains(role.RoleName) && !role.IsSelected)
                {
                    await _userManager.RemoveFromRoleAsync(user, role.RoleName);
                }
                else if (!userRoles.Contains(role.RoleName) && role.IsSelected)
                {
                    await _userManager.AddToRoleAsync(user, role.RoleName);
                }
            }
            return RedirectToPage("./Index");
        }
    }
}