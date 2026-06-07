using graduation_proj.Data;
using graduation_proj.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graduation_proj.Areas.Host.Controllers
{
    [Area("Host")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ACTION 1: View ALL reviews across all dishes belonging to this host
        // GET: /Host/Review/Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            var hostProfile = await _context.HostProfiles.FirstOrDefaultAsync(h => h.UserId == user.Id);
            if (hostProfile == null) return View(new List<Review>());

            // Get all reviews where the related dish belongs to this host
            var allReviews = await _context.Reviews
                .Include(r => r.Dish)
                .Include(r => r.Guest)
                .Where(r => r.Dish.HostProfileId == hostProfile.HostProfileId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(allReviews);
        }

        // ACTION 2: View reviews for ONE specific dish only
        // GET: /Host/Review/DishReviews?dishId=5
        public async Task<IActionResult> DishReviews(int dishId)
        {
            var dish = await _context.Dishes.FindAsync(dishId);
            if (dish == null) return NotFound();

            var reviews = await _context.Reviews
                .Include(r => r.Guest)
                .Where(r => r.DishId == dishId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.DishName = dish.Name;
            ViewBag.DishId = dishId;
            return View(reviews);
        }
    }
}