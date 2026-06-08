using graduation_proj.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace graduation_proj.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            // Sign the user out
            await _signInManager.SignOutAsync();

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }

            // No return url given, just reload this page (shows the signed-out message)
            return RedirectToPage();
        }
    }
}
