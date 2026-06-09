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
            // Only show dishes the admin has approved
            var query = _context.Dishes
                .Include(d => d.HostProfile)
                    .ThenInclude(h => h.User)
                .Where(d => d.IsApproved)
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

            // --- SORTING (rating is sorted later using the Reviews table directly) ---
            query = sort switch
            {
                "price" => query.OrderBy(d => d.Price),
                "rating" => query,
                _ => query.OrderByDescending(d => d.TimesBooked) // default: Most Booked
            };

            // --- DROPDOWN OPTIONS (built from the database) ---
            // Dropdown options only from hosts who have approved dishes
            ViewBag.Countries = await _context.Dishes
                .Where(d => d.IsApproved)
                .Select(d => d.HostProfile.Country)
                .Distinct()
                .ToListAsync();

            ViewBag.Cities = await _context.Dishes
                .Where(d => d.IsApproved)
                .Select(d => d.HostProfile.City)
                .Distinct()
                .ToListAsync();

            // Remember what the user picked so the form stays filled in
            ViewBag.SelectedCountry = country;
            ViewBag.SelectedCity = city;
            ViewBag.SelectedGuests = guests;
            ViewBag.Sort = sort;

            var dishes = await query.ToListAsync();

            // Load average ratings from the Reviews table (uses DishId column)
            var dishIds = dishes.Select(d => d.DishId).ToList();
            var ratingsByDish = await _context.Reviews
                .Where(r => r.DishId != null && dishIds.Contains(r.DishId.Value))
                .GroupBy(r => r.DishId!.Value)
                .Select(g => new { DishId = g.Key, Avg = g.Average(r => r.Rating) })
                .ToDictionaryAsync(x => x.DishId, x => x.Avg);

            ViewBag.RatingsByDish = ratingsByDish;

            if (sort == "rating")
            {
                dishes = dishes
                    .OrderByDescending(d => ratingsByDish.GetValueOrDefault(d.DishId))
                    .ToList();
            }

            return View(dishes);
        }

        public async Task<IActionResult> DishDetails(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.HostProfile)
                .ThenInclude(h => h.User)
                .FirstOrDefaultAsync(d => d.DishId == id && d.IsApproved);

            if (dish == null) return NotFound();

            return View(dish);
        }
    }
}