using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POEPart3.Data;
using POEPart3.Models;
using System.Diagnostics;
using System.Globalization;

namespace POEPart3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;

        }

        public IActionResult Index()
        {
            // Get the current user
            var userId = _userManager.GetUserId(User);

            // Retrieving data for the graph for the current user
            var studyRecords = _context.ModuleStudyRecords
                        .Include(m => m.Module)
                        .Where(m => m.UserId == userId)
                        .OrderBy(m => m.Date)
                        .ToList();

                /*
                 * Grouping study records by week
                 * then creating a new 'group' using 'select' statement.
                 * the group identifier is 'WeekNumber' using the 'group.Key'
                 * then getting total hours of all the records
                 * then getting the 'IdealHoursPerWeek' which is 'SelfStudyHoursPerWeek'
                 * then ordering then by the week number then saving that to a list
                 */
                var weeklyStudyData = studyRecords
                    .GroupBy(record => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(record.Date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday))
                    .Select(group => new
                    {
                        WeekNumber = group.Key,
                        TotalHours = group.Sum(record => record.HoursWorked),
                        IdealHours = group.First().Module.SelfStudyHoursPerWeek
                    })
                    .OrderBy(item => item.WeekNumber)
                    .ToList();

                // Preparing the data to be displayed on the graph but selecting each individual
                // items from the weeklyStudyData to a list each.
                var weekNumbers = weeklyStudyData.Select(item => item.WeekNumber).ToList();
                var totalHours = weeklyStudyData.Select(item => item.TotalHours).ToList();
                var idealHours = weeklyStudyData.Select(item => item.IdealHours).ToList();

            // Parsing the variables to ViewBag so the HTML can recognised/decode it.
                ViewBag.WeekNumbers = weekNumbers;
                ViewBag.TotalHours = totalHours;
                ViewBag.IdealHours = idealHours;

            // Returning to the Home Index View
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}