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
            if (ModelState.IsValid)
            {
                // Get the current logged in user
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account", new { area = "" });

                // Handle picture upload
                if (PlacePicture != null && PlacePicture.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/hosts");
                    Directory.CreateDirectory(uploadsFolder);
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(PlacePicture.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PlacePicture.CopyToAsync(stream);
                    }
                    model.PlacePicturePath = "/images/hosts/" + fileName;
                }

                // Link profile to user
                model.UserId = user.Id;

                // Save host profile
                _context.HostProfiles.Add(model);

                // Update user role to Host
                user.IsHost = true;
                await _userManager.RemoveFromRoleAsync(user, "Guest");
                await _userManager.AddToRoleAsync(user, "Host");
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Dish", new { area = "Host" });
            }
            return View(model);
        }
    }
}