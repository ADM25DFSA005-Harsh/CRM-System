

using CRMSystem.Data;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.EntityFrameworkCore;

using CRMSystem.Data;

using CRMSystem.Models;

using SalesAutomation.Models;

namespace SalesAutomation.Controllers

{

    public class SalesController : Controller

    {

        private readonly CrmDbContext _context;

        public SalesController(CrmDbContext context)

        {

            _context = context;

        }

        // Dashboard

        public async Task<IActionResult> SalesOverview()

        {

            var summary = new

            {

                TotalOpportunities = await _context.SalesOpportunity.CountAsync(),

                ClosingSoon = await _context.SalesOpportunity

                    .Where(o => o.ClosingDate.HasValue
&& o.ClosingDate.Value.Date >= DateTime.Today
&& o.ClosingDate.Value.Date <= DateTime.Today.AddDays(15)
&& (o.SalesStage == "Prospecting" || o.SalesStage == "Negotiation"))

                    .CountAsync(),

                Won = await _context.SalesOpportunity.CountAsync(o => o.SalesStage == "Won"),

                Lost = await _context.SalesOpportunity.CountAsync(o => o.SalesStage == "Lost")

            };

            var reminders = new

            {

                Message = $"You have {summary.ClosingSoon} opportunities closing in the next 15 days."

            };

            ViewBag.Summary = summary;

            ViewBag.Reminders = reminders;

            return View("SalesOverview");

        }

        // Leads Index

        [HttpGet]

        public async Task<IActionResult> LeadsList(string region, string status, string assignedRep)

        {

            var leadsQuery = _context.Leads.AsQueryable();

            if (!string.IsNullOrEmpty(region))

                leadsQuery = leadsQuery.Where(l => l.Region == region);

            if (!string.IsNullOrEmpty(status))

                leadsQuery = leadsQuery.Where(l => l.Status == status);

            if (!string.IsNullOrEmpty(assignedRep))

                leadsQuery = leadsQuery.Where(l => l.AssignedTo == assignedRep);

            ViewBag.AssignedReps = _context.Leads

                .Where(l => !string.IsNullOrEmpty(l.AssignedTo))

                .Select(r => new SelectListItem { Value = r.AssignedTo, Text = r.AssignedTo })

                .Distinct()

                .OrderBy(r => r.Text)

                .ToList();

            // Optional: Persist selected filters

            ViewBag.SelectedRegion = region;

            ViewBag.SelectedStatus = status;

            ViewBag.SelectedRep = assignedRep;

            var leads = await leadsQuery.OrderByDescending(l => l.CreatedDate).ToListAsync();

            return View("LeadsList", leads);

        }

        // Create Lead

        [HttpGet]

        public IActionResult Create()

        {

            ViewBag.Salutations = new List<string> { "Mr.", "Ms.", "Dr." };

            ViewBag.Regions = new List<string> { "North", "South", "East", "West" };

            ViewBag.Sources = _context.Leads

                .Where(l => !string.IsNullOrEmpty(l.Source))

                .Select(s => new SelectListItem { Value = s.Source, Text = s.Source })

                .Distinct()

                .OrderBy(s => s.Text)

                .ToList();

            ViewBag.Statuses = new List<string> { "New", "Contacted", "Qualified", "Disqualified" };

            ViewBag.AssignedReps = _context.Leads

                .Where(l => !string.IsNullOrEmpty(l.AssignedTo))

                .Select(r => new SelectListItem { Value = r.AssignedTo, Text = r.AssignedTo })

                .Distinct()

                .OrderBy(r => r.Text)

                .ToList();

            return View("CreateLead");

        }

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Lead lead)

        {

            if (ModelState.IsValid)

            {

                _context.Leads.Add(lead);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Lead created successfully!";

                return RedirectToAction(nameof(LeadsList));

            }

            return View("CreateLead", lead);

        }

        // Edit Lead

        [HttpGet]

        public async Task<IActionResult> Edit(int id)

        {

            var lead = await _context.Leads.FindAsync(id);

            if (lead == null) return NotFound();

            ViewBag.Salutations = new List<string> { "Mr.", "Ms.", "Dr." };

            ViewBag.Regions = new List<string> { "North", "South", "East", "West" };

            ViewBag.Sources = _context.Leads

                .Where(l => !string.IsNullOrEmpty(l.Source))

                .Select(s => new SelectListItem { Value = s.Source, Text = s.Source })

                .Distinct()

                .OrderBy(s => s.Text)

                .ToList();

            ViewBag.Statuses = new List<string> { "New", "Contacted", "Qualified", "Disqualified" };

            ViewBag.AssignedReps = _context.Leads

                .Where(l => !string.IsNullOrEmpty(l.AssignedTo))

                .Select(r => new SelectListItem { Value = r.AssignedTo, Text = r.AssignedTo })

                .Distinct()

                .OrderBy(r => r.Text)

                .ToList();

            return View("EditLead", lead);

        }

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(Lead lead)

        {

            if (!ModelState.IsValid)

            {

                ViewBag.Salutations = new List<string> { "Mr.", "Ms.", "Dr." };

                ViewBag.Regions = new List<string> { "North", "South", "East", "West" };

                ViewBag.Sources = _context.Leads

                    .Where(l => !string.IsNullOrEmpty(l.Source))

                    .Select(s => new SelectListItem { Value = s.Source, Text = s.Source })

                    .Distinct()

                    .OrderBy(s => s.Text)

                    .ToList();

                ViewBag.Statuses = new List<string> { "New", "Contacted", "Qualified", "Disqualified" };

                ViewBag.AssignedReps = _context.Leads

                    .Where(l => !string.IsNullOrEmpty(l.AssignedTo))

                    .Select(r => new SelectListItem { Value = r.AssignedTo, Text = r.AssignedTo })

                    .Distinct()

                    .OrderBy(r => r.Text)

                    .ToList();

                return View("EditLead", lead);

            }

            var existingLead = await _context.Leads.FindAsync(lead.LeadID);

            if (existingLead == null) return NotFound();

            existingLead.Salutation = lead.Salutation;

            existingLead.FirstName = lead.FirstName;

            existingLead.LastName = lead.LastName;

            existingLead.JobTitle = lead.JobTitle;

            existingLead.Email = lead.Email;

            existingLead.MobileNumber = lead.MobileNumber;

            existingLead.Region = lead.Region;

            existingLead.Source = lead.Source;

            existingLead.Status = lead.Status;

            existingLead.AssignedTo = lead.AssignedTo;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Lead updated successfully!";

            return RedirectToAction("Details", new { id = lead.LeadID });

        }

        // Lead Details

        [HttpGet]

        public async Task<IActionResult> Details(int id, string success)

        {

            var lead = await _context.Leads.FindAsync(id);

            if (lead == null) return NotFound();

            bool isConverted = false;

            if (lead.Status.Equals("Qualified", StringComparison.OrdinalIgnoreCase))

            {

                isConverted = await _context.SalesOpportunity.AnyAsync(o => o.LeadId == id);

            }

            ViewBag.IsConverted = isConverted;

            return View("LeadDetails", lead);

        }


        // Convert Lead to Deal

        [HttpPost]

        public IActionResult ConvertToDeal(int LeadId, decimal EstimatedValue)

        {

            var lead = _context.Leads.Find(LeadId);

            if (lead == null) return NotFound();

            var ExpectedCloseDate = DateTime.Now.AddDays(30);

            var deal = new SalesOpportunity

            {

                LeadId = lead.LeadID,

                Lead = lead,

                EstimatedValue = EstimatedValue,

                ClosingDate = ExpectedCloseDate,

                AssignedRep = lead.AssignedTo,

                SalesStage = "Prospecting"

            };

            _context.SalesOpportunity.Add(deal);

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Lead converted to deal successfully.";

            return RedirectToAction("DealsList");

        }

        // Deals List

        public async Task<IActionResult> DealsList(string? campaign, string? region, string? stage, string? AssignedTo, string? valueSort, string? dateSort, string? status)

        {

            var query = _context.SalesOpportunity

                .Include(o => o.Lead)

                .AsQueryable();

            if (!string.IsNullOrEmpty(campaign))

                query = query.Where(o => o.Lead.Source == campaign);

            if (!string.IsNullOrEmpty(stage))

                query = query.Where(o => o.SalesStage == stage);

            if (!string.IsNullOrEmpty(AssignedTo))

                query = query.Where(o => o.AssignedRep == AssignedTo);

            if (!string.IsNullOrEmpty(status))

                query = query.Where(o => o.Lead.Status == status);

            IOrderedQueryable<SalesOpportunity> orderedQuery;

            if (valueSort == "asc")

                orderedQuery = query.OrderBy(o => o.EstimatedValue);

            else if (valueSort == "desc")

                orderedQuery = query.OrderByDescending(o => o.EstimatedValue);

            else

                orderedQuery = query.OrderBy(o => o.OpportunityID);

            if (dateSort == "latest")

                orderedQuery = orderedQuery.ThenByDescending(o => o.ClosingDate);

            else if (dateSort == "oldest")

                orderedQuery = orderedQuery.ThenBy(o => o.ClosingDate);

            var rawDeals = await orderedQuery.ToListAsync();

            var deals = rawDeals.Select(o => new

            {

                o.OpportunityID,

                CustomerName = string.Join(" ", new[] { o.Lead?.Salutation, o.Lead?.FirstName, o.Lead?.LastName }.Where(s => !string.IsNullOrEmpty(s))),

                CampaignName = o.Lead?.Source,

                o.SalesStage,

                o.EstimatedValue,

                o.ClosingDate,

                AssignedRep = o.Lead?.AssignedTo

            }).ToList();

            ViewBag.CampaignList = _context.Leads

                .Select(l => l.Source)

                .Distinct()

                .Select(c => new SelectListItem { Value = c, Text = c })

                .ToList();

            ViewBag.StageList = new List<SelectListItem>

            {

                new SelectListItem { Value = "Prospecting", Text = "Prospecting" },

                new SelectListItem { Value = "Negotiation", Text = "Negotiation" },

                new SelectListItem { Value = "Won", Text = "Won" },

                new SelectListItem { Value = "Lost", Text = "Lost" }

            };

            ViewBag.RepList = _context.Leads

                .Where(l => !string.IsNullOrEmpty(l.AssignedTo))

                .Select(r => new SelectListItem { Value = r.AssignedTo, Text = r.AssignedTo })

                .Distinct()

                .OrderBy(r => r.Text)

                .ToList();

            ViewBag.StatusList = _context.Leads

                .Select(l => l.Status)

                .Distinct()

                .Select(s => new SelectListItem { Value = s, Text = s })

                .ToList();

            ViewBag.Deals = deals;

            return View("DealsList");

        }

        [HttpPost]

        public async Task<IActionResult> UpdateStage(int OpportunityID, string SalesStage)

        {

            var deal = await _context.SalesOpportunity.FindAsync(OpportunityID);

            if (deal == null) return NotFound();

            if (deal.SalesStage == "Won" || deal.SalesStage == "Lost")

            {

                TempData["ErrorMessage"] = "Cannot change stage after deal is closed.";

                return RedirectToAction("DealsList");

            }

            deal.SalesStage = SalesStage;

            if (SalesStage == "Won" || SalesStage == "Lost")

                deal.ClosingDate = DateTime.Now;

            else if (SalesStage == "Negotiation")

                deal.ClosingDate = DateTime.Now.AddDays(15);

            _context.Update(deal);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Stage updated successfully.";

            return RedirectToAction("DealsList");

        }

        // AJAX Logging Methods

        [HttpPost]

        public IActionResult AddEmailLog(int leadId, string title)

        {

            var lead = _context.Leads.Find(leadId);

            if (lead == null) return NotFound();

            var today = DateTime.Now.ToString("dd-MMM");

            var newLog = $"{today}: {title}";

            lead.EmailHistory = string.IsNullOrEmpty(lead.EmailHistory)

                ? newLog

                : lead.EmailHistory + "|" + newLog;

            _context.SaveChanges();

            return Json(new { success = true, history = lead.EmailHistory.Split('|') });

        }

        [HttpPost]

        public IActionResult AddCallLog(int leadId, string title)

        {

            var lead = _context.Leads.Find(leadId);

            if (lead == null) return NotFound();

            var today = DateTime.Now.ToString("dd-MMM");

            var newLog = $"{today}: {title}";

            lead.CallHistory = string.IsNullOrEmpty(lead.CallHistory)

                ? newLog

                : lead.CallHistory + "|" + newLog;

            _context.SaveChanges();

            return Json(new { success = true, history = lead.CallHistory.Split('|') });

        }

        [HttpPost]

        public IActionResult AddNoteLog(int leadId, string title, string body)

        {

            var lead = _context.Leads.Find(leadId);

            if (lead == null) return NotFound();

            var formattedBody = body.Replace("\n", "<br/>");

            var newLog = $"<strong>{title}</strong><br/>{formattedBody}";

            lead.Notes = string.IsNullOrEmpty(lead.Notes)

                ? newLog

                : lead.Notes + "|" + newLog;

            _context.SaveChanges();

            return Json(new { success = true, history = lead.Notes.Split('|') });

        }

        [HttpPost]

        public IActionResult AddTaskLog(int leadId, string title, string description, string priority, string date)

        {

            var lead = _context.Leads.Find(leadId);

            if (lead == null) return NotFound();

            var formattedDesc = description.Replace("\n", "<br/>");

            var priorityIcon = priority switch

            {

                "High" => "🔴 High",

                "Medium" => "🟠 Medium",

                "Low" => "🟢 Low",

                _ => priority

            };

            var newLog = $"<div><strong>{title}</strong> &nbsp; <span>{priorityIcon}</span></div>" +

                         $"<div>{date}</div>" +

                         $"<div>{formattedDesc}</div>";

            lead.Tasks = string.IsNullOrEmpty(lead.Tasks)

                ? newLog

                : lead.Tasks + "|" + newLog;

            _context.SaveChanges();

            return Json(new { success = true, history = lead.Tasks.Split('|') });

        }

    }

}
