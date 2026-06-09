using graduation_proj.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace graduation_proj.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DishController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DishController(ApplicationDbContext context)
        {
            _context = context;
        }

        // List dishes waiting for admin approval (newest first)
        public async Task<IActionResult> Index()
        {
            var dishes = await _context.Dishes
                .Include(d => d.HostProfile)
                .ThenInclude(h => h.User)
                .Where(d => !d.IsApproved)
                .OrderByDescending(d => d.DishId)
                .ToListAsync();

            return View(dishes);
        }

        // Approve a dish so it shows on the guest home page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return NotFound();

            dish.IsApproved = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Delete a dish the admin does not want to approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var dish = await _context.Dishes.FindAsync(id);
            if (dish == null) return NotFound();

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
