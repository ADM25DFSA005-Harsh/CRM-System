

using CRMSystem.Data;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.EntityFrameworkCore;

using CRMSystem.Data;

using CRMSystem.Models;

using SalesAutomation.Models;

namespace SalesAutomation.Controllers

{
    [ApiController]
    [Route("api/[controller]")]

    public class SalesController : Controller

    {

        private readonly CrmDbContext _context;

        public SalesController(CrmDbContext context)

        {

            _context = context;

        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetSalesOverview()
        {
            var summary = new
            {
                TotalOpportunities = await _context.SalesOpportunity.CountAsync(),
                ClosingSoon = await _context.SalesOpportunity
                    .Where(o => o.ClosingDate.HasValue &&
                                o.ClosingDate.Value.Date >= DateTime.Today &&
                                o.ClosingDate.Value.Date <= DateTime.Today.AddDays(15) &&
                                (o.SalesStage == "Prospecting" || o.SalesStage == "Negotiation"))
                    .CountAsync(),
                Won = await _context.SalesOpportunity.CountAsync(o => o.SalesStage == "Won"),
                Lost = await _context.SalesOpportunity.CountAsync(o => o.SalesStage == "Lost")
            };

            var reminders = new
            {
                Message = $"You have {summary.ClosingSoon} opportunities closing in the next 15 days."
            };

            return Ok(new { summary, reminders });
        }

        // Retrieves filtered list of leads
        [HttpGet("leads")]
        public async Task<IActionResult> GetLeadsList([FromQuery] string? region, [FromQuery] string? status, [FromQuery] string? assignedRep)
        {
            var leadsQuery = _context.Leads.AsQueryable();

            if (!string.IsNullOrEmpty(region))
                leadsQuery = leadsQuery.Where(l => l.Region == region);
            if (!string.IsNullOrEmpty(status))
                leadsQuery = leadsQuery.Where(l => l.Status == status);
            if (!string.IsNullOrEmpty(assignedRep))
                leadsQuery = leadsQuery.Where(l => l.AssignedTo == assignedRep);

            var leads = await leadsQuery.OrderByDescending(l => l.CreatedDate).ToListAsync();
            return Ok(leads);
        }

        // Creates a new lead
        [HttpPost("lead")]
        public async Task<IActionResult> CreateLead([FromBody] Lead lead)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Leads.Add(lead);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Lead created successfully!", lead });
        }

        // Updates an existing lead
        [HttpPut("lead/{id}")]
        public async Task<IActionResult> UpdateLead(int id, [FromBody] Lead lead)
        {
            var existingLead = await _context.Leads.FindAsync(id);
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
            return Ok(new { message = "Lead updated successfully!", lead });
        }

        // Retrieves the data of the lead using Id
        [HttpGet("lead/{id}")]
        public async Task<IActionResult> GetLeadDetails(int id)
        {
            var lead = await _context.Leads.FindAsync(id);
            if (lead == null) return NotFound();

            bool isConverted = lead.Status?.Equals("Qualified", StringComparison.OrdinalIgnoreCase) == true &&
                await _context.SalesOpportunity.AnyAsync(o => o.LeadId == id);

            return Ok(new { lead, isConverted });
        }

        // Converts a lead to a deal
        [HttpPost("convert")]
        public IActionResult ConvertToDeal([FromQuery] int LeadId, [FromQuery] decimal EstimatedValue)
        {
            var lead = _context.Leads.Find(LeadId);
            if (lead == null) return NotFound();

            var deal = new SalesOpportunity
            {
                LeadId = lead.LeadID,
                Lead = lead,
                EstimatedValue = EstimatedValue,
                ClosingDate = DateTime.Now.AddDays(30),
                AssignedRep = lead.AssignedTo,
                SalesStage = "Prospecting"
            };

            _context.SalesOpportunity.Add(deal);
            _context.SaveChanges();

            return Ok(new { message = "Lead converted to deal successfully.", deal });
        }

        // Retrieves filtered list of deals
        [HttpGet("deals")]
        public async Task<IActionResult> GetDealsList([FromQuery] string? campaign, [FromQuery] string? stage, [FromQuery] string? AssignedTo, [FromQuery] string? valueSort, [FromQuery] string? dateSort, [FromQuery] string? status)
        {
            var query = _context.SalesOpportunity.Include(o => o.Lead).AsQueryable();

            if (!string.IsNullOrEmpty(campaign))
                query = query.Where(o => o.Lead.Source == campaign);
            if (!string.IsNullOrEmpty(stage))
                query = query.Where(o => o.SalesStage == stage);
            if (!string.IsNullOrEmpty(AssignedTo))
                query = query.Where(o => o.AssignedRep == AssignedTo);
            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Lead.Status == status);

            IOrderedQueryable<SalesOpportunity> orderedQuery = valueSort switch
            {
                "asc" => query.OrderBy(o => o.EstimatedValue),
                "desc" => query.OrderByDescending(o => o.EstimatedValue),
                _ => query.OrderBy(o => o.OpportunityID)
            };

            orderedQuery = dateSort switch
            {
                "latest" => orderedQuery.ThenByDescending(o => o.ClosingDate),
                "oldest" => orderedQuery.ThenBy(o => o.ClosingDate),
                _ => orderedQuery
            };

            var deals = await orderedQuery.ToListAsync();
            return Ok(deals);
        }

        // Updates the stage of an existing deal
        [HttpPut("deal/{id}/stage")]
        public async Task<IActionResult> UpdateStage(int id, [FromQuery] string SalesStage)
        {
            var deal = await _context.SalesOpportunity.FindAsync(id);
            if (deal == null) return NotFound();

            if (deal.SalesStage == "Won" || deal.SalesStage == "Lost")
                return BadRequest("Cannot change stage after deal is closed.");

            deal.SalesStage = SalesStage;
            deal.ClosingDate = SalesStage switch
            {
                "Won" or "Lost" => DateTime.Now,
                "Negotiation" => DateTime.Now.AddDays(15),
                _ => deal.ClosingDate
            };

            _context.Update(deal);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Stage updated successfully.", deal });
        }

        [HttpPost("lead/{id}/email")]
        public IActionResult AddEmailLog(int id, [FromQuery] string title)
        {
            var lead = _context.Leads.Find(id);
            if (lead == null) return NotFound();

            var today = DateTime.Now.ToString("dd-MMM");
            var newLog = $"{today}: {title}";

            lead.EmailHistory = string.IsNullOrEmpty(lead.EmailHistory) ? newLog : lead.EmailHistory + "|" + newLog;
            _context.SaveChanges();

            return Ok(new { success = true, history = lead.EmailHistory.Split('|') });
        }

        [HttpPost("lead/{id}/call")]
        public IActionResult AddCallLog(int id, [FromQuery] string title)
        {
            var lead = _context.Leads.Find(id);
            if (lead == null) return NotFound();

            var today = DateTime.Now.ToString("dd-MMM");
            var newLog = $"{today}: {title}";

            lead.CallHistory = string.IsNullOrEmpty(lead.CallHistory) ? newLog : lead.CallHistory + "|" + newLog;
            _context.SaveChanges();

            return Ok(new { success = true, history = lead.CallHistory.Split('|') });
        }

        [HttpPost("lead/{id}/note")]
        public IActionResult AddNoteLog(int id, [FromQuery] string title, [FromQuery] string body)
        {
            var lead = _context.Leads.Find(id);
            if (lead == null) return NotFound();

            var formattedBody = body.Replace("\n", " < br /> ");
            var newLog = $"<strong>{title}</strong><br/>{formattedBody}";

            lead.Notes = string.IsNullOrEmpty(lead.Notes) ? newLog : lead.Notes + "|" + newLog;
            _context.SaveChanges();

            return Ok(new { success = true, history = lead.Notes.Split('|') });
        }

        [HttpPost("lead/{id}/task")]
        public IActionResult AddTaskLog(int id, [FromQuery] string title, [FromQuery] string description, [FromQuery] string priority, [FromQuery] string date)
        {
            var lead = _context.Leads.Find(id);
            if (lead == null) return NotFound();

            var formattedDesc = description.Replace("\n", " < br /> ");
            var priorityIcon = priority switch
            {
                "High" => "🔴 High",
                "Medium" => "🟠 Medium",
                "Low" => "🟢 Low",
                _ => priority
            };

            var newLog = $"<div><strong>{title}</strong> &nbsp; <span>{priorityIcon}</span></div><div>{date}</div><div>{formattedDesc}</div>";

            lead.Tasks = string.IsNullOrEmpty(lead.Tasks) ? newLog : lead.Tasks + "|" + newLog;
            _context.SaveChanges();

            return Ok(new { success = true, history = lead.Tasks.Split('|') });
        }

    }

}
