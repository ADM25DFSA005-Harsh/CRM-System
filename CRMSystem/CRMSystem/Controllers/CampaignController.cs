using MarketingAutomation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRMSystem.Data;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MarketingAutomation.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CampaignController : ControllerBase
    {
        private readonly CrmDbContext _context;

        public CampaignController(CrmDbContext context)
        {
            _context = context;
        }



        //Retrieves a list of all Campaigns.

        //HTTP 200 OK with the list of campaigns.

        // GET: api/Marketing

        [HttpGet]

        public async Task<ActionResult<IEnumerable<Campaign>>> GetCampaigns()

        {

            UpdateCampaignStatusesAndImpressions();

            var campaign = await _context.Campaign.ToListAsync();

            return Ok(campaign);

        }


        //Retrieves a single Campaign by ID.

        //HTTP 200 OK with the Campaign, or HTTP 404 Not Found.

        // GET: api/Campaigns/5

        [HttpGet("{id}")]

        public async Task<ActionResult<Campaign>> GetCampaign(int id)

        {

            var campaign = await _context.Campaign.FindAsync(id);

            if (campaign == null)

            {

                return NotFound();

            }

            return Ok(campaign);

        }



        //Creates a new Campaign.

        //HTTP 201 CreatedAtAction or HTTP 400 Bad Request.

        // POST: api/Campaigns

        [HttpPost]

        public async Task<IActionResult> CreateCampaign([FromBody] Campaign campaign)

        {

            // ModelState validation is handled automatically by [ApiController], 

            // but we explicitly check and return 400 Bad Request if invalid.

            if (!ModelState.IsValid)

            {

                return BadRequest(ModelState);

            }



            var random = new Random();

            campaign.Impressions = random.Next(1000, 50001);

            if (campaign.Status == "Scheduled")

            {

                campaign.Impressions = 0;

            }


            _context.Campaign.Add(campaign);

            await _context.SaveChangesAsync();

            // Use CreatedAtAction for a 201 response, pointing the client to the new resource location.

            return CreatedAtAction(nameof(GetCampaign), new { id = campaign.CampaignID }, campaign);

        }


        //Updates an existing Campaign.

        //HTTP 204 No Content, HTTP 400 Bad Request, or HTTP 404 Not Found.

        // PUT: api/Campaigns/5

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateCampaign(int id, [FromBody] Campaign campaign)

        {

            if (id != campaign.CampaignID || !ModelState.IsValid)

            {

                return BadRequest();

            }

            // Attach the entity and mark it as modified for EF Core to track changes.

            _context.Entry(campaign).State = EntityState.Modified;

            try

            {

                await _context.SaveChangesAsync();

            }

            catch (DbUpdateConcurrencyException)

            {

                if (!_context.Campaign.Any(e => e.CampaignID == id))

                {

                    return NotFound(); // Campaign doesn't exist

                }

                else

                {

                    throw;

                }

            }

            // HTTP 204 is the standard response for a successful, non-data-returning update operation.

            return NoContent();

        }



        //Deletes a Campaign by ID.

        //HTTP 204 No Content or HTTP 404 Not Found.

        // DELETE: api/Campaigns/5

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteCampaign(int id)

        {

            var campaign = await _context.Campaign.FindAsync(id);

            if (campaign == null)

            {

                return NotFound();

            }

            _context.Campaign.Remove(campaign);

            await _context.SaveChangesAsync();

            // HTTP 204 is the standard response for a successful deletion.

            return NoContent();

        }


        private void UpdateCampaignStatusesAndImpressions()

        {

            var today = DateTime.Now;

            var campaignsToUpdate = _context.Campaign.ToList();

            var random = new Random();

            foreach (var campaign in campaignsToUpdate)

            {

                // Determine New Status

                if (campaign.StartDate <= today && campaign.EndDate >= today)

                    campaign.Status = "Active";

                else if (campaign.EndDate < today)

                    campaign.Status = "Completed";

                else if (campaign.StartDate > today)

                    campaign.Status = "Scheduled";

                // Update Impressions based on the new status

                if (campaign.Status == "Scheduled")

                    campaign.Impressions = 0;

                // Add a random component to active/completed impressions for simulation

                else if (campaign.Status == "Active")

                    campaign.Impressions += random.Next(500, 1000); // Simulate *new* impressions

                else if (campaign.Status == "Completed")

                    campaign.Impressions = campaign.Impressions; // Keep the final number

            }

            _context.SaveChanges();

        }


        //Retrieves a filtered list of campaigns based on query parameters.

        //Filter by predefined section (today, upcoming, completed, filter).

        //Filter by specific status and type

        // GET: api/Campaigns/track? section = upcoming & status = Scheduled

        [HttpGet("track")]

        public async Task<ActionResult<IEnumerable<Campaign>>> TrackCampaigns(

            [FromQuery] string section,

            [FromQuery] string status,

            [FromQuery] string type)

        {

            var today = DateTime.Today;

            var campaigns = _context.Campaign.AsQueryable();

            // Apply filters based on query parameters

            switch (section?.ToLower())

            {

                case "today":

                    campaigns = campaigns.Where(c => c.Status == "Active");

                    break;

                case "upcoming":

                    campaigns = campaigns.Where(c => c.Status == "Scheduled");

                    break;

                case "completed":

                    campaigns = campaigns.Where(c => c.Status == "Completed");

                    break;

                case "filter":

                    if (!string.IsNullOrEmpty(status))

                        campaigns = campaigns.Where(c => c.Status == status);

                    if (!string.IsNullOrEmpty(type))

                        campaigns = campaigns.Where(c => c.Type == type);

                    break;

            }

            var results = await campaigns.ToListAsync();

            // Angular will use this JSON array to display the filtered list.

            return Ok(results);

        }



        //Gets a summary of campaign budgets grouped by type.

        [HttpGet("budget")]

        public async Task<IActionResult> GetCampaignBudgetsSummary()

        {

            var groupedBudgets = await _context.Campaign

               .Where(c => !string.IsNullOrEmpty(c.Type))

               .GroupBy(c => c.Type)

               .Select(g => new

               {

                   Type = g.Key,

                   TotalBudget = g.Sum(c => c.Budget)

               })

               .ToListAsync();

            // Returns a structured JSON object for chart/summary display in Angular.

            return Ok(new { TotalBudgetsByType = groupedBudgets });

        }

        //Gets a list of campaigns belonging to a specific type.

        //HTTP 200 OK with a list of campaigns.

        // GET: api/Campaigns/budget/Email

        [HttpGet("budget/{type}")]

        public async Task<IActionResult> GetCampaignsByType(string type)

        {

            var campaignsInType = await _context.Campaign

                .Where(c => c.Type == type)

                .ToListAsync();

            return Ok(campaignsInType);

        }

        //Calculates and retrieves campaign performance metrics for a selected type.

        //HTTP 200 OK with performance metrics, types list, and top campaign.

        // GET: api/Campaigns/performance?selectedType=Email

        [HttpGet("performance")]

        public async Task<IActionResult> GetCampaignPerformance([FromQuery] string selectedType)

        {

            // Load campaign types for Angular dropdowns

            var campaignTypes = await _context.Campaign

                .Select(c => c.Type)

                .Distinct()

                .ToListAsync();

            var campaigns = new List<object>();

            object topCampaign = null;

            if (!string.IsNullOrEmpty(selectedType))

            {

                // Get campaigns and calculate performance metrics, including navigation properties for responses

                campaigns = await _context.Campaign

                    .Where(c => c.Type == selectedType)

                    .Select(c => new

                    {

                        c.CampaignName,

                        c.Impressions,

                        TotalResponses = c.CustomerResponse.Count(),

                        AverageRating = c.CustomerResponse.Average(r => (double?)r.Rating)

                    })

                    .Cast<object>() // Cast to object list to handle the anonymous type

                    .ToListAsync();

                // Identify top campaign by response count

                topCampaign = campaigns

                    .OrderByDescending(c => ((dynamic)c).TotalResponses)

                    .FirstOrDefault();

            }

            // Return a single JSON object containing all data needed by the Angular view.

            return Ok(new

            {

                Types = campaignTypes,

                Campaigns = campaigns,

                TopCampaign = topCampaign

            });

        }





    }
}
