using graduation_proj.Data;
using graduation_proj.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Join()
        {
            var user = await _userManager.GetUserAsync(User);

            // If the user already has a host profile, they are already a host.
            // Send them to their dishes instead of showing the join form again.
            if (user != null && _context.HostProfiles.Any(h => h.UserId == user.Id))
            {
                return RedirectToAction("Index", "Dish", new { area = "Host" });
            }

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

            // If they already have a host profile, don't let them create a second one
            if (_context.HostProfiles.Any(h => h.UserId == user.Id))
            {
                return RedirectToAction("Index", "Dish", new { area = "Host" });
            }

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

        // Show edit profile form (only for hosts who already joined)
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            var profile = await _context.HostProfiles
                .FirstOrDefaultAsync(h => h.UserId == user.Id);

            if (profile == null) return RedirectToAction(nameof(Join));

            ViewBag.FullName = user.FullName;
            return View(profile);
        }

        // Save edited profile
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HostProfile model, IFormFile? PlacePicture, string fullName)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            var profile = await _context.HostProfiles
                .FirstOrDefaultAsync(h => h.HostProfileId == model.HostProfileId && h.UserId == user.Id);

            if (profile == null) return NotFound();

            ModelState.Remove("UserId");
            ModelState.Remove("User");
            ModelState.Remove("Dishes");

            if (ModelState.IsValid)
            {
                // Update text fields
                profile.Country = model.Country;
                profile.City = model.City;
                profile.Location = model.Location;
                profile.PhoneNumber = model.PhoneNumber;
                profile.Bio = model.Bio;

                // Update personal photo only if a new file was uploaded
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
                    profile.PlacePicturePath = "/images/hosts/" + fileName;
                }

                // Update display name on the user account
                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    user.FullName = fullName;
                }

                await _context.SaveChangesAsync();

                TempData["ProfileUpdated"] = true;
                return RedirectToAction("Index", "Dish", new { area = "Host" });
            }

            ViewBag.FullName = fullName;
            return View(model);
        }
    }
}