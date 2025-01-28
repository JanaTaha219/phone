using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication6.Models;

namespace WebApplication6.Pages.UserRoles
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public List<UserInfo> Users { get; set; }
        public async Task OnGetAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            Users = new List<UserInfo>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Users.Add(new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                });
            }
        }
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            await _userManager.DeleteAsync(user);
            return RedirectToPage();
        }
    }
}
