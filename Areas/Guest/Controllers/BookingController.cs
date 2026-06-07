using graduation_proj.Data;
using graduation_proj.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graduation_proj.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Guest/Booking/Create?dishId=1
        public async Task<IActionResult> Create(int dishId)
        {
            var dish = await _context.Dishes.FindAsync(dishId);
            if (dish == null) return NotFound();

            ViewBag.Dish = dish;
            return View();
        }

        // POST: /Guest/Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            booking.GuestId = user.Id;
            booking.Status = BookingStatus.Pending;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyBookings));
        }

        // GET: /Guest/Booking/MyBookings
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            var bookings = await _context.Bookings
                .Include(b => b.Dish)
                .Where(b => b.GuestId == user.Id)
                .ToListAsync();

            return View(bookings);
        }

        // POST: /Guest/Booking/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            // Safety check: ensure the current user actually owns this booking before letting them cancel it
            var user = await _userManager.GetUserAsync(User);
            if (booking.GuestId != user?.Id) return Unauthorized();

            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyBookings));
        }
    }
}