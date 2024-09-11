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
using POEPart3;
using POEPart3.Data;

namespace POEPart3.Controllers
{
    [Authorize]
    public class ModuleStudyRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        //new toasts
        private readonly INotyfService _notyf;

        public ModuleStudyRecordsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, INotyfService notyf)
        {
            _context = context;
            _userManager = userManager;
            _notyf = notyf;
        }

        // GET: ModuleStudyRecords
        public async Task<IActionResult> Index()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            var applicationDbContext = _context.ModuleStudyRecords
                .Include(m => m.Module)
                .Where(m => m.UserId == user.Id)
                .ToListAsync();

            return View(await applicationDbContext);
        }

        // GET: ModuleStudyRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ModuleStudyRecords == null)
            {
                return NotFound();
            }

            var moduleStudyRecord = await _context.ModuleStudyRecords
                .Include(m => m.Module)
                .FirstOrDefaultAsync(m => m.RecordId == id);
            if (moduleStudyRecord == null)
            {
                return NotFound();
            }

            return View(moduleStudyRecord);
        }

        // GET: ModuleStudyRecords/Create
        public async Task<IActionResult> CreateAsync()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            ViewData["ModuleId"] = new SelectList(_context.Modules.Where(m => m.UserId == user.Id), "ModuleId", "ModuleId");
            ViewData["ModuleCode"] = new SelectList(_context.Modules.Where(m => m.UserId == user.Id), "Code", "Code");
            return View();
        }

        //POST: ModuleStudyRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModuleCode,Date,HoursWorked,HoursLeft,UserId,ModuleId")] ModuleStudyRecord moduleStudyRecord)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            moduleStudyRecord.UserId = user.Id;

            // Fetching the date to use for calculations
            DateTime selectedDate = moduleStudyRecord.Date;

            // Fetching the module entered by the user to use for calculations
            Module selectedModule = _context.Modules
                .Where(m => m.Code == moduleStudyRecord.ModuleCode && m.UserId == user.Id)
                .First();

            moduleStudyRecord.Module = selectedModule;
            moduleStudyRecord.ModuleId = selectedModule.ModuleId;

            // Fetching the semester entered by the user to use for calculations
            Semester currentSemester = _context.Semesters
                .Where(s => s.SemesterId == selectedModule.SemesterId)
                .First();

            if (moduleStudyRecord is ModuleStudyRecord)
            {
                // Check if the selected date is within the semester's date range
                if (selectedDate >= currentSemester.StartDate && selectedDate <= currentSemester.EndDate)
                {
                    // Calculating the start and end dates of the current week
                    DateTime weekStartDate = selectedDate.Date.AddDays(-(int)selectedDate.DayOfWeek);
                    DateTime weekEndDate = weekStartDate.AddDays(6);

                    // Calculate the total hours worked for the current week
                    int totalHoursThisWeek = _context.ModuleStudyRecords
                        .Where(record => record.Date >= weekStartDate && record.Date <= weekEndDate && record.ModuleId == selectedModule.ModuleId)
                        .Sum(record => record.HoursWorked);

                    // Calculating the remaining self-study hours for the current week
                    int remainingSelfStudyHours = selectedModule.SelfStudyHoursPerWeek - totalHoursThisWeek;

                    // Calculating the hours that will exceed the allowed self-study hours
                    int hoursExceedingLimit = remainingSelfStudyHours - moduleStudyRecord.HoursWorked;

                    // Checking if adding the new record would exceed the allowed self-study hours
                    if (hoursExceedingLimit < 0)
                    {
                        ModelState.AddModelError("HoursWorked", $"Adding {moduleStudyRecord.HoursWorked} hours would exceed the allowed self-study hours for this week. " +
                            $"Remaining hours: {remainingSelfStudyHours}. You will exceed by {hoursExceedingLimit} hours.");

                        return RedirectToAction(nameof(Index));
                    }

                    // Adding HoursLeft to record
                    moduleStudyRecord.HoursLeft = selectedModule.SelfStudyHoursPerWeek - totalHoursThisWeek - moduleStudyRecord.HoursWorked;
                    // Setting ModuleCode manually
                    moduleStudyRecord.ModuleCode = selectedModule.Code;

                    // Save changes
                    _context.Add(moduleStudyRecord);
                    await _context.SaveChangesAsync();
                    if (_notyf != null)
                    {
                        _notyf.Success("Record added to database!");
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //ModelState.AddModelError("Date", $"Selected date is outside the semester's date range." +
                    //    $"\nPlease select a date within: {currentSemester.StartDate:dd/MM/yyyy} & {currentSemester.EndDate:dd/MM/yyyy}");
                    if (_notyf != null)
                    {
                        _notyf.Error("Selected date is outside the semester`s date range", 4);
                    }
                    return RedirectToAction(nameof(Create));
                }

            }

            ViewData["ModuleId"] = new SelectList(_context.Modules.Where(m => m.UserId == user.Id), "ModuleId", "ModuleId", moduleStudyRecord.ModuleId);
            ViewData["ModuleCode"] = new SelectList(_context.Modules.Where(m => m.UserId == user.Id), "Code", "Code", moduleStudyRecord.ModuleCode);
            return View(moduleStudyRecord);
        }


        // GET: ModuleStudyRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (id == null || _context.ModuleStudyRecords == null)
            {
                return NotFound();
            }

            var moduleStudyRecord = await _context.ModuleStudyRecords.FindAsync(id);
            if (moduleStudyRecord == null)
            {
                return NotFound();
            }
            ViewData["ModuleId"] = new SelectList(_context.Modules.Where(m => m.UserId == user.Id), "ModuleId", "ModuleId", moduleStudyRecord.ModuleId);
            ViewData["ModuleCode"] = new SelectList(_context.Modules.Where(m => m.UserId == user.Id), "Code", "Code", moduleStudyRecord.ModuleCode);
            return View(moduleStudyRecord);
        }

        // POST: ModuleStudyRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecordId,ModuleCode,Date,HoursWorked,HoursLeft,UserId,ModuleId")] ModuleStudyRecord moduleStudyRecord)
        {
            // Get current user
            var user = await _userManager.GetUserAsync(User);

            if (id != moduleStudyRecord.RecordId)
            {
                return NotFound();
            }

            if (moduleStudyRecord is ModuleStudyRecord)
            {
                try
                {
                    moduleStudyRecord.UserId = user.Id;

                    moduleStudyRecord.ModuleId = _context.Modules
                        .Where(m => m.Code == moduleStudyRecord.ModuleCode && m.UserId == user.Id)
                        .Select(m => m.ModuleId)
                        .First();

                    _context.Update(moduleStudyRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleStudyRecordExists(moduleStudyRecord.RecordId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            if (_notyf != null)
            {
                _notyf.Success("Record has been updated!", 2);
            }
            ViewData["ModuleId"] = new SelectList(_context.Modules.Where(m => m.UserId == user.Id), "ModuleId", "ModuleId", moduleStudyRecord.ModuleId);
            ViewData["ModuleCode"] = new SelectList(_context.Modules.Where(m => m.UserId == user.Id), "Code", "Code", moduleStudyRecord.ModuleCode);
            return View(moduleStudyRecord);
        }

        // GET: ModuleStudyRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ModuleStudyRecords == null)
            {
                return NotFound();
            }

            var moduleStudyRecord = await _context.ModuleStudyRecords
                .Include(m => m.Module)
                .FirstOrDefaultAsync(m => m.RecordId == id);
            if (moduleStudyRecord == null)
            {
                return NotFound();
            }

            return View(moduleStudyRecord);
        }

        // POST: ModuleStudyRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ModuleStudyRecords == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ModuleStudyRecords'  is null.");
            }
            var moduleStudyRecord = await _context.ModuleStudyRecords.FindAsync(id);
            if (moduleStudyRecord != null)
            {
                _context.ModuleStudyRecords.Remove(moduleStudyRecord);
            }

            await _context.SaveChangesAsync();
            if (_notyf != null)
            {
                _notyf.Success("Record has been deleted!", 2);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleStudyRecordExists(int id)
        {
            return (_context.ModuleStudyRecords?.Any(e => e.RecordId == id)).GetValueOrDefault();
        }
    }
}
