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

        // Guest: show review form for a dish
        public async Task<IActionResult> Create(int dishId)
        {
            var dish = await _context.Dishes.FindAsync(dishId);
            if (dish == null) return NotFound();
            ViewBag.DishName = dish.Name;
            ViewBag.DishId = dishId;
            return View();
        }

        // Guest: save review
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            review.GuestId = user.Id;
            review.CreatedAt = DateTime.Now;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyBookings", "Booking");
        }

        // Show all reviews for a dish
        public async Task<IActionResult> DishReviews(int dishId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Guest)
                .Where(r => r.DishId == dishId)
                .ToListAsync();

            ViewBag.DishId = dishId;
            return View(reviews);
        }
    }
}