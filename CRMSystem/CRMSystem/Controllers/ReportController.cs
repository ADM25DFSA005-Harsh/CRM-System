using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRMSystem.Data;
using CRMSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace CRMSystem.Controllers
{

    //[Authorize(Roles = "Manager,Admin")] // Only Manager and Admin can access Report actions
    public class ReportController : Controller
    {
        private readonly CrmDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReportController(CrmDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        //[AllowAnonymous]
        public IActionResult LoginRedirect()
        {
            return Redirect("/Identity/Account/Login");
        }
        // ✅ New Action: Role-based dashboard redirection
        //[AllowAnonymous]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Dashboard()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Content("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin") || roles.Contains("Manager"))
            {
                // Show dashboard view with module options
                return View("Dashboard");
            }
            //else if (roles.Contains("CustomerAssociate"))
            //{
            //    return RedirectToAction("DisplayProfiles", "CustomerProfile");
            //}
            else if (roles.Contains("SalesAssociate"))
            {
                return RedirectToAction("SalesOverview", "Sales");
            }
            else if (roles.Contains("MarketingAssociate"))
            {
                return RedirectToAction("Index", "Campaign");
            }
            else if (roles.Contains("SupportAgent"))
            {
                return RedirectToAction("DisplayInTableForm", "CustomerSupport");
            }
            else if (roles.Contains("ReportAnalyst"))
            {
                //return View("ModuleReport");
                return RedirectToAction("ModuleReport", "Report");
            }

            return Content("You do not have access to any modules.");
        }



        // ✅ Existing Report actions (unchanged)
        
        public IActionResult Index()
        {
            var reports = _context.Reports.ToList();
            return View(reports);
        }

        public IActionResult Details(int id)
        {
            var report = _context.Reports.FirstOrDefault(r => r.ReportID == id);
            if (report == null)
            {
                return NotFound();
            }
            return View(report);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Report report)
        {
            if (ModelState.IsValid)
            {
                _context.Reports.Add(report);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(report);
        }

        public IActionResult Edit(int id)
        {
            var report = _context.Reports.FirstOrDefault(r => r.ReportID == id);
            if (report == null)
            {
                return NotFound();
            }
            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Report updatedReport)
        {
            if (id != updatedReport.ReportID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Reports.Update(updatedReport);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(updatedReport);
        }

        public IActionResult Delete(int id)
        {
            var report = _context.Reports.FirstOrDefault(r => r.ReportID == id);
            if (report == null)
            {
                return NotFound();
            }
            return View(report);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var report = _context.Reports.FirstOrDefault(r => r.ReportID == id);
            if (report != null)
            {
                _context.Reports.Remove(report);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Manager,Report")]
        public IActionResult ModuleReport()
        {
            //ViewBag.SelectedModule = module ?? "None";

            // Later: fetch filtered data from DB based on module
            // For now: use hardcoded data in the view

            return View();
        }

        
    }
}
