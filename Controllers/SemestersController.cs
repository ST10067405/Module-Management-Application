using System;
using System.Collections.Generic;
using System.Windows;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POEPart3;
using POEPart3.Data;
using POEPart3.Models;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace POEPart3.Controllers
{
    [Authorize]
    public class SemestersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        //new toasts
        private readonly INotyfService _notyf;

        public SemestersController(ApplicationDbContext context, UserManager<IdentityUser> userManager, INotyfService notyf)
        {
            _context = context;
            _userManager = userManager;
            _notyf = notyf ?? throw new ArgumentException(nameof(notyf));
        }

        // GET: Semesters
        public async Task<IActionResult> Index()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            // Retrieve and display semesters for the current user
            var userSemesters = await _context.Semesters
                .Where(m => m.UserId == user.Id)
                .ToListAsync();

            return View(userSemesters);
        }

        // GET: Semesters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Semesters == null)
            {
                return NotFound();
            }

            var semester = await _context.Semesters
                .FirstOrDefaultAsync(m => m.SemesterId == id);

            if (semester == null)
            {
                return NotFound();
            }

            return View(semester);
        }

        // GET: Semesters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Semesters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Bind attribute protects from overposting attacks
        public async Task<IActionResult> Create([Bind("SemesterId,Name,NumberOfWeeks,StartDate,EndDate,UserId")] Semester semester)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            //setting the user that created this semester
            semester.UserId = user.Id;

            //setting the EndDate
            semester.EndDate = semester.StartDate.AddDays(7 * semester.NumberOfWeeks);

            //checking if the semester object is matching the Semester model
            if (semester is Semester)
            {
                _context.Add(semester);
                await _context.SaveChangesAsync();

                if (_notyf != null)
                {
                    _notyf.Success("Semester added to database!", 2);
                    _notyf.Success("You may start to capture your modules", 2);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(semester);
        }

        // GET: Semesters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Semesters == null)
            {
                return NotFound();
            }

            var semester = await _context.Semesters.FindAsync(id);
            if (semester == null)
            {
                return NotFound();
            }
            return View(semester);
        }

        // POST: Semesters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SemesterId,Name,NumberOfWeeks,StartDate,EndDate,UserId")] Semester semester)
        {
            if (id != semester.SemesterId)
            {
                return NotFound();
            }

            if (semester is Semester)
            {
                try
                {
                    // Get the current user
                    var user = await _userManager.GetUserAsync(User);

                    //setting the user that created this semester
                    semester.UserId = user.Id;

                    _context.Update(semester);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SemesterExists(semester.SemesterId))
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
                    _notyf.Success("Semester has been updated!", 2);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(semester);
        }

        // GET: Semesters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Semesters == null)
            {
                return NotFound();
            }

            var semester = await _context.Semesters
                .FirstOrDefaultAsync(m => m.SemesterId == id);
            if (semester == null)
            {
                return NotFound();
            }

            return View(semester);
        }

        // POST: Semesters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Semesters == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Semesters'  is null.");
            }
            var semester = await _context.Semesters.FindAsync(id);
            if (semester != null)
            {
                _context.Semesters.Remove(semester);
            }

            await _context.SaveChangesAsync();
            if (_notyf != null)
            {
                _notyf.Success("Semester has been deleted!", 2);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SemesterExists(int id)
        {
            return (_context.Semesters?.Any(e => e.SemesterId == id)).GetValueOrDefault();
        }
    }
}
