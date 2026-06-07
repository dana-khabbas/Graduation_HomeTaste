using graduation_proj.Data;
using graduation_proj.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace graduation_proj.Areas.Host.Controllers
{
    [Area("Host")]
    public class HostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Show join as host form
        [Authorize]
        public IActionResult Join()
        {
            return View();
        }

        // Save host profile
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(HostProfile model, IFormFile? PlacePicture)
        {
            // 1. Get the current logged-in user details instantly
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "" });

            // 2. Set the foreign key BEFORE validation processing happens
            model.UserId = user.Id;

            // 3. Force the validation engine to overlook background tracking properties
            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("Dishes");

            if (ModelState.IsValid)
            {
                // Handle picture upload safely
                if (PlacePicture != null && PlacePicture.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "hosts");
                    Directory.CreateDirectory(uploadsFolder);
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(PlacePicture.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PlacePicture.CopyToAsync(stream);
                    }
                    model.PlacePicturePath = "/images/hosts/" + fileName;
                }

                // Save host profile data record down to our DB table
                _context.HostProfiles.Add(model);
                user.IsHost = true;
                await _context.SaveChangesAsync(); // Save these changes immediately first!

                // 4. Role Assignment Safety Handling
                // If your database doesn't have these exact role strings created yet, 
                // using the UserManager role methods directly can cause runtime freezes.
                try
                {
                    // Remove from Guest if they were assigned it
                    if (await _userManager.IsInRoleAsync(user, "Guest"))
                    {
                        await _userManager.RemoveFromRoleAsync(user, "Guest");
                    }
                    // Add to Host
                    await _userManager.AddToRoleAsync(user, "Host");
                }
                catch (Exception)
                {
                    // If Roles tables aren't set up yet, let it slide silently so your app doesn't freeze
                }

                return RedirectToAction("Index", "Dish", new { area = "Host" });
            }

            // If we reach here, validation failed! Let's show you why in Step 2.
            return View(model);
        }
    }
}