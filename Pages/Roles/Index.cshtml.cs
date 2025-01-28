using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication6.Pages.Roles
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
