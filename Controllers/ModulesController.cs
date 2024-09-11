using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModulesLibrary;
using POEPart3;
using POEPart3.Data;

namespace POEPart3.Controllers
{
    [Authorize]
    public class ModulesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        //new toasts
        private readonly INotyfService _notyf;

        public ModulesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, INotyfService notyf)
        {
            _context = context;
            _userManager = userManager;
            _notyf = notyf;
        }

        // GET: Modules
        public async Task<IActionResult> Index()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            var userModules = _context.Modules
                .Include(m => m.Semester)
                .Where(m => m.UserId == user.Id)
                .ToListAsync();

            return View(await userModules);
        }

        // GET: Modules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            var @module = await _context.Modules
                .Include(m => m.Semester)
                .Where(m => m.UserId == user.Id)
                .FirstOrDefaultAsync(m => m.ModuleId == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // GET: Modules/Create
        public async Task<IActionResult> CreateAsync()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            ViewData["SemesterId"] = new SelectList(_context.Semesters.Where(m => m.UserId == user.Id), "SemesterId", "Name");
            return View();
        }

        // POST: Modules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModuleId,Code,Name,Credits,ClassHoursPerWeek,SelfStudyHoursPerWeek,UserId,SemesterId")] Module @module)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            module.UserId = user.Id;

            //fetching the numberofweeks for the module's semester
            int NumberOfWeeks = _context.Semesters
                .Where(s => s.SemesterId == module.SemesterId)
                .Select(s => s.NumberOfWeeks)
                .FirstOrDefault();

            @module.SelfStudyHoursPerWeek = ModuleManagement.SelfStudyCalc(@module.Credits, NumberOfWeeks, @module.ClassHoursPerWeek);

            if (module is Module)
            {
                _context.Add(@module);
                await _context.SaveChangesAsync();
                if (_notyf != null)
                {
                    _notyf.Success("Module added to database!", 2);
                    _notyf.Success("You may start to capture your Module Records", 2);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "SemesterId", "Name", @module.SemesterId);
            return View(@module);
        }

        // GET: Modules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }
            ViewData["SemesterId"] = new SelectList(_context.Semesters.Where(m => m.UserId == user.Id), "SemesterId", "Name", @module.SemesterId);
            return View(@module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ModuleId,Code,Name,Credits,ClassHoursPerWeek,SelfStudyHoursPerWeek,UserId,SemesterId")] Module @module)
        {
            if (id != @module.ModuleId)
            {
                return NotFound();
            }

            if (module is Module)
            {
                try
                {
                    // Get the current user
                    var user = await _userManager.GetUserAsync(User);
                    module.UserId = user.Id;

                    _context.Update(@module);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(@module.ModuleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (_notyf != null)
                {
                    _notyf.Success("Module has been updated!", 2);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "SemesterId", "Name", @module.SemesterId);
            return View(@module);
        }

        // GET: Modules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Modules == null)
            {
                return NotFound();
            }

            var @module = await _context.Modules
                .Include(m => m.Semester)
                .FirstOrDefaultAsync(m => m.ModuleId == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Modules == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Modules'  is null.");
            }
            var @module = await _context.Modules.FindAsync(id);
            if (@module != null)
            {
                _context.Modules.Remove(@module);
            }
            
            await _context.SaveChangesAsync();
            if (_notyf != null)
            {
                _notyf.Success("Module has been deleted!", 2);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleExists(int id)
        {
          return (_context.Modules?.Any(e => e.ModuleId == id)).GetValueOrDefault();
        }
    }
}
