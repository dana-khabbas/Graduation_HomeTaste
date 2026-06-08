using graduation_proj.Data;
using graduation_proj.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graduation_proj.Areas.Host.Controllers
{
    [Area("Host")]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Host/Booking/ManageBookings
        // Displays bookings for dishes belonging exclusively to the logged-in Host
        public async Task<IActionResult> ManageBookings()
        {
            // 1. Get current logged-in Host user
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            // 2. Fetch the Host Profile associated with this user
            var hostProfile = await _context.HostProfiles
                .FirstOrDefaultAsync(h => h.UserId == user.Id);

            if (hostProfile == null)
            {
                // If they are logged in but haven't made a host profile, return an empty list or error
                return View(new List<Booking>());
            }

            // 3. Find bookings linked to dishes where Dish.HostProfileId matches this Host
            var hostBookings = await _context.Bookings
                .Include(b => b.Dish)   // To show dish details (Name, Price)
                .Include(b => b.Guest)  // To show guest details (FullName, Email)
                .Where(b => b.Dish.HostProfileId == hostProfile.HostProfileId)
                .OrderByDescending(b => b.VisitDate)
                .ToListAsync();

            return View(hostBookings);
        }

        // POST: /Host/Booking/Confirm/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Dish)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null) return NotFound();

            // Safety validation: verify this booking belongs to a dish owned by the current host
            var user = await _userManager.GetUserAsync(User);
            var hostProfile = await _context.HostProfiles.FirstOrDefaultAsync(h => h.UserId == user.Id);
            if (hostProfile == null || booking.Dish.HostProfileId != hostProfile.HostProfileId)
            {
                return Unauthorized();
            }

            booking.Status = BookingStatus.Confirmed;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageBookings));
        }

        // POST: /Host/Booking/Deny/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deny(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Dish)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null) return NotFound();

            // Safety validation: verify this booking belongs to a dish owned by the current host
            var user = await _userManager.GetUserAsync(User);
            var hostProfile = await _context.HostProfiles.FirstOrDefaultAsync(h => h.UserId == user.Id);
            if (hostProfile == null || booking.Dish.HostProfileId != hostProfile.HostProfileId)
            {
                return Unauthorized();
            }

            // Denying simply marks the booking as Cancelled (it stays in the list)
            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageBookings));
        }
    }
}