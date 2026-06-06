using graduation_proj.Data;
using graduation_proj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graduation_proj.Areas.Host.Controllers
{
    [Area("Host")]
    public class DishController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DishController(ApplicationDbContext context)
        {
            _context = context;
        }

        // List all dishes for this host
        public async Task<IActionResult> Index()
        {
            var dishes = await _context.Dishes
                .Include(d => d.HostProfile)
                .ToListAsync();
            return View(dishes);
        }

        // Show add form
        public IActionResult Create()
        {
            return View();
        }

        // Save new dish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Dish dish)
        {
            if (ModelState.IsValid)
            {
                _context.Dishes.Add(dish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dish);
        }

        // Show edit form
        public async Task<IActionResult> Edit(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return NotFound();
            return View(dish);
        }

        // Save edited dish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Dish dish)
        {
            if (id != dish.DishId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(dish);
                await _context.SaveChangesAsync();
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