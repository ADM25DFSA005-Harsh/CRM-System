using CRMSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRMSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportApiController : ControllerBase
    {
        private readonly CrmDbContext _context;

        public ReportApiController(CrmDbContext context)
        {
            _context = context;
        }

        // MARKETING FILTERS
        [HttpGet("marketing/filters")]
        public async Task<IActionResult> GetMarketingFilters()
        {
            var types = await _context.Campaign
                .Select(c => c.Type)
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            var statuses = await _context.Campaign
                .Select(c => c.Status)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            var scheduleTypes = await _context.Campaign
                .Select(c => c.ScheduleType)
                .Where(st => !string.IsNullOrEmpty(st))
                .Distinct()
                .OrderBy(st => st)
                .ToListAsync();

            return Ok(new { Types = types, Statuses = statuses, ScheduleTypes = scheduleTypes });
        }

        // SALES FILTERS
        [HttpGet("sales/filters")]

        public async Task<IActionResult> GetSalesFilters()
        {
            var stages = await _context.SalesOpportunity
                .Select(s => s.SalesStage)
                .Distinct()
                .ToListAsync();

            var reps = await _context.SalesOpportunity
                .Select(s => s.AssignedRep)
                .Distinct()
                .ToListAsync();

            return Ok(new { SalesStages = stages, AssignedReps = reps });
        }


        // SUPPORT FILTERS
        [HttpGet("support/filters")]
        public async Task<IActionResult> GetSupportFilters()
        {
            var statuses = await _context.SupportTicket.Select(t => t.Status).Distinct().ToListAsync();
            var agents = await _context.SupportTicket.Select(t => t.AssignedAgent).Distinct().ToListAsync();

            return Ok(new { Statuses = statuses, Agents = agents });
        }

        //// MARKETING DASHBOARD (with filters)
        //[HttpGet("marketing")]
        //public async Task<IActionResult> GetMarketingDashboard([FromQuery] string? type, [FromQuery] string? status, [FromQuery] string? scheduleType)
        //{
        //    var query = _context.Campaign.AsQueryable();

        //    if (!string.IsNullOrEmpty(type))
        //        query = query.Where(c => c.Type != null && c.Type.ToLower() == type.ToLower());

        //    if (!string.IsNullOrEmpty(status))
        //        query = query.Where(c => c.Status != null && c.Status.ToLower() == status.ToLower());

        //    if (!string.IsNullOrEmpty(scheduleType))
        //        query = query.Where(c => c.ScheduleType != null && c.ScheduleType.ToLower() == scheduleType.ToLower());

        //    var byType = await query
        //        .GroupBy(c => c.Type)
        //        .Select(g => new { Label = g.Key ?? "Unknown", Count = g.Count() })
        //        .ToListAsync();

        //    var budget = await query
        //        .GroupBy(c => c.Type)
        //        .Select(g => new { Label = g.Key ?? "Unknown", Value = g.Sum(x => x.Budget) })
        //        .ToListAsync();

        //    var statusBreakdown = await query
        //        .GroupBy(c => c.Status)
        //        .Select(g => new { Label = g.Key ?? "Unknown", Count = g.Count() })
        //        .ToListAsync();

        //    var trend = await query
        //        .Where(c => c.StartDate != null)
        //        .GroupBy(c => new { Month = c.StartDate.Month, Year = c.StartDate.Year })
        //        .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, Impressions = g.Sum(x => x.Impressions) })
        //        .OrderBy(x => x.Year).ThenBy(x => x.Month)
        //        .Take(6)
        //        .ToListAsync();

        //    return Ok(new
        //    {
        //        ByType = byType,
        //        Budget = budget,
        //        Status = statusBreakdown,
        //        Trend = trend
        //    });
        //}
        [HttpGet("marketing")]
        public async Task<IActionResult> GetMarketingDashboard([FromQuery] string? type, [FromQuery] string? status, [FromQuery] string? scheduleType)
        {
            var query = _context.Campaign.AsQueryable();


            if (!string.IsNullOrEmpty(type)) query = query.Where(c => c.Type == type);
            if (!string.IsNullOrEmpty(status)) query = query.Where(c => c.Status == status);
            if (!string.IsNullOrEmpty(scheduleType)) query = query.Where(c => c.ScheduleType == scheduleType);
            //if (!string.IsNullOrEmpty(type))
            //    query = query.Where(c => c.Type.ToLower() == type.ToLower());
            //if (!string.IsNullOrEmpty(status))
            //    query = query.Where(c => c.Status.ToLower() == status.ToLower());
            //if (!string.IsNullOrEmpty(scheduleType))
            //    query = query.Where(c => c.ScheduleType.ToLower() == scheduleType.ToLower());

            var byType = await query.GroupBy(c => c.Type).Select(g => new { Label = g.Key, Count = g.Count() }).ToListAsync();
            var budget = await query.GroupBy(c => c.Type).Select(g => new { Label = g.Key, Value = g.Sum(x => x.Budget) }).ToListAsync();
            var statusBreakdown = await query.GroupBy(c => c.Status).Select(g => new { Label = g.Key, Count = g.Count() }).ToListAsync();
            var trend = await query.Where(c => c.StartDate != null)
                .GroupBy(c => new { Month = c.StartDate.Month, Year = c.StartDate.Year })
                .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, Impressions = g.Sum(x => x.Impressions) })
                .OrderBy(x => x.Year).ThenBy(x => x.Month).Take(6).ToListAsync();

            return Ok(new { ByType = byType, Budget = budget, Status = statusBreakdown, Trend = trend });
            //return Ok(new
            //{
            //    ByType = byType ?? new List<object>(),
            //    Budget = budget ?? new List<object>(),
            //    Status = statusBreakdown ?? new List<object>(),
            //    Trend = trend ?? new List<object>()
            //});
        }

        // SALES DASHBOARD (with filters)
        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesDashboard([FromQuery] string? salesStage, [FromQuery] string? assignedRep)
        {
            var query = _context.SalesOpportunity.AsQueryable();

            if (!string.IsNullOrEmpty(salesStage))
                query = query.Where(s => s.SalesStage == salesStage);

            if (!string.IsNullOrEmpty(assignedRep))
                query = query.Where(s => s.AssignedRep == assignedRep);

            var byStage = await query
                .GroupBy(s => s.SalesStage)
                .Select(g => new { Label = g.Key, Count = g.Count() })
                .ToListAsync();

            var revenue = await query
                .GroupBy(s => s.SalesStage)
                .Select(g => new { Label = g.Key, Value = g.Sum(x => x.EstimatedValue) })
                .ToListAsync();

            var repBreakdown = await query
                .GroupBy(s => s.AssignedRep)
                .Select(g => new { Label = g.Key, Count = g.Count() })
                .ToListAsync();

            var trend = await query
                .Where(s => s.ClosingDate.HasValue)
                .GroupBy(s => new { Month = s.ClosingDate.Value.Month, Year = s.ClosingDate.Value.Year })
                .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, Revenue = g.Sum(x => x.EstimatedValue) })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .Take(6)
                .ToListAsync();

            return Ok(new { ByType = byStage, Budget = revenue, Status = repBreakdown, Trend = trend });
        }

        // SUPPORT DASHBOARD (with filters)
        [HttpGet("support")]
        public async Task<IActionResult> GetSupportDashboard([FromQuery] string? status, [FromQuery] string? agent)
        {
            var query = _context.SupportTicket.AsQueryable();

            if (!string.IsNullOrEmpty(status)) query = query.Where(t => t.Status == status);
            if (!string.IsNullOrEmpty(agent)) query = query.Where(t => t.AssignedAgent == agent);

            var byIssue = await query.GroupBy(t => t.Issue).Select(g => new { Label = g.Key, Count = g.Count() }).ToListAsync();
            var agentDist = await query.GroupBy(t => t.AssignedAgent).Select(g => new { Label = g.Key, Count = g.Count() }).ToListAsync();
            var statusBreakdown = await query.GroupBy(t => t.Status).Select(g => new { Label = g.Key, Count = g.Count() }).ToListAsync();

            return Ok(new { ByType = byIssue, Budget = agentDist, Status = statusBreakdown });
        }
    }
}
