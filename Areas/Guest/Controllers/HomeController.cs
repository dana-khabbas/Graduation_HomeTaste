using graduation_proj.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graduation_proj.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string country, string city, int? guests, string sort)
        {
            // Start with all dishes (plus their host info)
            var query = _context.Dishes
                .Include(d => d.HostProfile)
                .ThenInclude(h => h.User)
                .AsQueryable();

            // --- SEARCH FILTERS ---
            // Only filter when a real value was chosen (the "Anywhere"/"All Cities" options mean no filter)
            if (!string.IsNullOrEmpty(country) && country != "Anywhere")
            {
                query = query.Where(d => d.HostProfile.Country == country);
            }

            if (!string.IsNullOrEmpty(city) && city != "All Cities")
            {
                query = query.Where(d => d.HostProfile.City == city);
            }

            if (guests.HasValue)
            {
                // Show dishes that can serve at least the chosen number of guests
                query = query.Where(d => d.ServesPersons >= guests.Value);
            }

            // --- SORTING ---
            query = sort switch
            {
                "price" => query.OrderBy(d => d.Price),
                "rating" => query.OrderByDescending(d => d.Reviews.Any() ? d.Reviews.Average(r => r.Rating) : 0),
                _ => query.OrderByDescending(d => d.TimesBooked) // default: Most Booked
            };

            // --- DROPDOWN OPTIONS (built from the database) ---
            ViewBag.Countries = await _context.HostProfiles
                .Where(h => h.Country != null)
                .Select(h => h.Country)
                .Distinct()
                .ToListAsync();

            ViewBag.Cities = await _context.HostProfiles
                .Where(h => h.City != null)
                .Select(h => h.City)
                .Distinct()
                .ToListAsync();

            // Remember what the user picked so the form stays filled in
            ViewBag.SelectedCountry = country;
            ViewBag.SelectedCity = city;
            ViewBag.SelectedGuests = guests;
            ViewBag.Sort = sort;

            var dishes = await query.ToListAsync();
            return View(dishes);
        }

        public async Task<IActionResult> DishDetails(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.HostProfile)
                .ThenInclude(h => h.User)
                .FirstOrDefaultAsync(d => d.DishId == id);

            if (dish == null) return NotFound();

            return View(dish);
        }
    }
}