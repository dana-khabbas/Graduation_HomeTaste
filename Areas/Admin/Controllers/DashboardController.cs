using graduation_proj.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graduation_proj.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Count totals from the database
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalHosts = await _context.HostProfiles.CountAsync();
            ViewBag.TotalDishes = await _context.Dishes.CountAsync();
            ViewBag.TotalBookings = await _context.Bookings.CountAsync();

            // Top 3 most booked dishes
            ViewBag.TopDishes = await _context.Dishes
                .Include(d => d.HostProfile)
                .ThenInclude(h => h.User)
                .OrderByDescending(d => d.TimesBooked)
                .Take(3)
                .ToListAsync();

            // Latest 3 bookings
            ViewBag.LatestBookings = await _context.Bookings
                .Include(b => b.Dish)
                .Include(b => b.Guest)
                .OrderByDescending(b => b.BookingId)
                .Take(3)
                .ToListAsync();

            return View();
        }
    }
}
