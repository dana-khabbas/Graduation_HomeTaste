using graduation_proj.Data;
using graduation_proj.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graduation_proj.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Guest/Review/Create?dishId=5
        public async Task<IActionResult> Create(int dishId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            var dish = await _context.Dishes.FindAsync(dishId);
            if (dish == null) return NotFound();

            // One review per guest per dish — block if they already reviewed this dish
            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.GuestId == user.Id && r.DishId == dishId);
            if (alreadyReviewed)
            {
                TempData["ReviewError"] = "You have already reviewed this dish.";
                return RedirectToAction("MyBookings", "Booking", new { area = "Guest" });
            }

            ViewBag.Dish = dish;

            var review = new Review
            {
                DishId = dishId
            };

            return View(review);
        }

        // POST: /Guest/Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            review.GuestId = user.Id;
            review.CreatedAt = DateTime.Now;

            ModelState.Remove("Guest");
            ModelState.Remove("Dish");

            if (ModelState.IsValid)
            {
                // Double-check on submit so nobody can post twice
                var alreadyReviewed = await _context.Reviews
                    .AnyAsync(r => r.GuestId == user.Id && r.DishId == review.DishId);
                if (alreadyReviewed)
                {
                    TempData["ReviewError"] = "You have already reviewed this dish.";
                    return RedirectToAction("MyBookings", "Booking", new { area = "Guest" });
                }

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                // Used on My Bookings to show the success popup
                TempData["ReviewSuccess"] = true;
                return RedirectToAction("MyBookings", "Booking", new { area = "Guest" });
            }

            ViewBag.Dish = await _context.Dishes.FindAsync(review.DishId);
            return View(review);
        }
    }
}