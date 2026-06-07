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

        public async Task<IActionResult> Index()
        {
            var dishes = await _context.Dishes
                .Include(d => d.HostProfile)
                .ThenInclude(h => h.User)
                .ToListAsync();
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