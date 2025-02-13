using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BugTracker1._2025.Data;
using BugTracker1._2025.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker1._2025.Controllers
{
    [Authorize] // Ensure only logged-in users can access
    public class BugController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BugController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bug
        public async Task<IActionResult> Index()
        {
            var bugs = await _context.Bugs.Include(b => b.AssignedToUser).ToListAsync();
            return View(bugs);
        }

        // GET: Bug/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var bug = await _context.Bugs
                .Include(b => b.AssignedToUser)
                .Include(b => b.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bug == null) return NotFound();

            return View(bug);
        }

        // GET: Bug/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var developerRoleId = await _context.Roles
                .Where(r => r.Name == "Developer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            ViewBag.Users = new SelectList(await _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == developerRoleId))
                .ToListAsync(), "Id", "UserName");

            return View();
        }

        // POST: Bug/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Reporter,Developer")]
        public async Task<IActionResult> Create([Bind("Title,Description,Priority,AssignedToUserId")] Bug bug)
        {
            if (ModelState.IsValid)
            {
                bug.Status = Status.Open;
                bug.CreatedAt = DateTime.UtcNow;

                //  Automatically assign to a Developer if none is selected
                if (string.IsNullOrEmpty(bug.AssignedToUserId))
                {
                    var developerRoleId = await _context.Roles
                        .Where(r => r.Name == "Developer")
                        .Select(r => r.Id)
                        .FirstOrDefaultAsync();

                    var availableDevelopers = await _context.Users
                        .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == developerRoleId))
                        .ToListAsync();

                    if (availableDevelopers.Any())
                    {
                        bug.AssignedToUserId = availableDevelopers.OrderBy(u => Guid.NewGuid()).First().Id; // Randomly assign
                        bug.AssignedToUser = await _context.Users.FindAsync(bug.AssignedToUserId);
                    }
                }

                _context.Add(bug);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var devRoleId = await _context.Roles
                .Where(r => r.Name == "Developer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            ViewBag.Users = new SelectList(await _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == devRoleId))
                .ToListAsync(), "Id", "UserName");

            return View(bug);
        }

        // GET: Bug/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bug = await _context.Bugs.FindAsync(id);
            if (bug == null) return NotFound();

            var developerRoleId = await _context.Roles
                .Where(r => r.Name == "Developer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            ViewBag.Users = new SelectList(await _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == developerRoleId))
                .ToListAsync(), "Id", "UserName", bug.AssignedToUserId);

            return View(bug);
        }

        // POST: Bug/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Status,Priority,AssignedToUserId")] Bug bug)
        {
            if (id != bug.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(bug.AssignedToUserId))
                    {
                        bug.AssignedToUser = await _context.Users.FindAsync(bug.AssignedToUserId);
                    }

                    _context.Update(bug);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Bugs.Any(e => e.Id == bug.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            var developerRoleId = await _context.Roles
                .Where(r => r.Name == "Developer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            ViewBag.Users = new SelectList(await _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == developerRoleId))
                .ToListAsync(), "Id", "UserName", bug.AssignedToUserId);

            return View(bug);
        }

        // Allow Developers & Admins to Update Bug Status
        [Authorize(Roles = "Developer,Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, Status newStatus)
        {
            var bug = await _context.Bugs.FindAsync(id);
            if (bug == null) return NotFound();

            bug.Status = newStatus;
            _context.Update(bug);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Bug/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bug = await _context.Bugs.FirstOrDefaultAsync(m => m.Id == id);
            if (bug == null) return NotFound();

            return View(bug);
        }

        // POST: Bug/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bug = await _context.Bugs.FindAsync(id);
            if (bug != null)
            {
                _context.Bugs.Remove(bug);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BugExists(int id)
        {
            return _context.Bugs.Any(e => e.Id == id);
        }
    }
}