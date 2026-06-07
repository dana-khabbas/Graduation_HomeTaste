using graduation_proj.Data;
using graduation_proj.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graduation_proj.Areas.Host.Controllers
{
    [Area("Host")]
    public class DishController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Inject the UserManager so we can track the logged-in Host
        public DishController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // FIXED: Only list dishes that belong to THIS logged-in host
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            var hostProfile = await _context.HostProfiles.FirstOrDefaultAsync(h => h.UserId == user.Id);
            if (hostProfile == null) return View(new List<Dish>());

            var dishes = await _context.Dishes
                .Where(d => d.HostProfileId == hostProfile.HostProfileId)
                .ToListAsync();

            return View(dishes);
        }

        // Show add form
        public IActionResult Create()
        {
            return View();
        }

        // FIXED: Save new dish with the current host's ID attached
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dish dish, IFormFile? dishPicture)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            var hostProfile = await _context.HostProfiles.FirstOrDefaultAsync(h => h.UserId == user.Id);
            if (hostProfile == null)
            {
                ModelState.AddModelError("", "You need a Host Profile before creating a dish.");
                return View(dish);
            }

            // --- FILE UPLOAD LOGIC START ---
            if (dishPicture != null && dishPicture.Length > 0)
            {
                // 1. Create a distinct folder path inside wwwroot/images/dishes
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "dishes");

                // Ensure that the physical directory exists on the machine
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // 2. Generate a unique file name using a Guid to prevent duplicate overrides
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(dishPicture.FileName);
                string fileSavePath = Path.Combine(uploadsFolder, uniqueFileName);

                // 3. Save the file stream down to your disk
                using (var fileStream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await dishPicture.CopyToAsync(fileStream);
                }

                // 4. Save the relative URL path into the string property of your entity
                dish.DishPicturePath = "/images/dishes/" + uniqueFileName;
            }
            // --- FILE UPLOAD LOGIC END ---

            dish.HostProfileId = hostProfile.HostProfileId;
            dish.HostProfile = hostProfile;

            // Clean up navigation validations
            ModelState.Remove("HostProfile");
            ModelState.Remove("Bookings");
            ModelState.Remove("Reviews");

            if (ModelState.IsValid)
            {
                _context.Dishes.Add(dish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(dish);
        }

        // Show edit form
        [HttpGet] 
        public async Task<IActionResult> Edit(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return NotFound();
            return View(dish);
        }

        // Save edited dish
        [HttpPost] // <-- THIS WAS MISSING AND SAYS IT HANDLES POST ACTIONS
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Dish dish, IFormFile? dishPicture)
        {
            if (id != dish.DishId) return NotFound();

            // --- FILE UPLOAD LOGIC START ---
            if (dishPicture != null && dishPicture.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "dishes");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(dishPicture.FileName);
                string fileSavePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(fileSavePath, FileMode.Create))
                {
                    await dishPicture.CopyToAsync(fileStream);
                }

                dish.DishPicturePath = "/images/dishes/" + uniqueFileName;
            }
            // --- FILE UPLOAD LOGIC END ---

            // Ignore navigation properties on update validation as well
            ModelState.Remove("HostProfile");
            ModelState.Remove("Bookings");
            ModelState.Remove("Reviews");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Dishes.Any(e => e.DishId == dish.DishId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dish);
        }

        // Show delete confirmation
        public async Task<IActionResult> Delete(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.HostProfile)
                .FirstOrDefaultAsync(d => d.DishId == id);
            if (dish == null) return NotFound();
            return View(dish);
        }

        // Confirm delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish != null) _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // View dish details
        public async Task<IActionResult> Details(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.HostProfile)
                .FirstOrDefaultAsync(d => d.DishId == id);
            if (dish == null) return NotFound();
            return View(dish);
        }
    }
}