using MarketingAutomation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRMSystem.Data;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MarketingAutomation.Controllers
{
    public class CampaignController : Controller
    {
        private readonly CrmDbContext _context;

        public CampaignController(CrmDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        // GET: /Campaigns/ or /Campaigns/ManageCampaign
        // This is the main view action, handling the list and initial page load.
        [HttpGet]
        public IActionResult ManageCampaign(string actionType = "view")
        {
            if (actionType == "view")
            {
                UpdateCampaignStatusesAndImpressions();
            }

            ViewBag.ActionType = actionType;
            ViewBag.Campaigns = _context.Campaign.ToList();
            return View("ManageCampaign", new Campaign());
        }

        // --- CREATE OPERATIONS ---

        // GET: /Campaigns/Create - Displays the blank "Create Campaign" form
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ActionType = "create";
            ViewBag.Campaigns = _context.Campaign.ToList();
            return View("ManageCampaign", new Campaign());
        }

        // POST: /Campaigns/Create - Handles the form submission for creating a new campaign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                // Auto-generate Impressions for new campaign
                var random = new Random();
                campaign.Impressions = random.Next(1000, 50001);

                // Set Impressions to 0 for Scheduled campaigns (overrides the random value)
                if (campaign.Status == "Scheduled")
                {
                    campaign.Impressions = 0;
                }

                _context.Campaign.Add(campaign);
                _context.SaveChanges();

                TempData["Message"] = $"Campaign '{campaign.CampaignName}' created successfully.";

                // Post-Redirect-Get pattern: Redirect to the main list view
                return RedirectToAction("Index", new { actionType = "view" });
            }

            // If invalid, return to the 'ManageCampaign' view, explicitly showing the 'create' form
            ViewBag.ActionType = "create";
            ViewBag.Campaigns = _context.Campaign.ToList();
            return View("ManageCampaign", campaign);
        }

        // --- EDIT OPERATIONS (using a standard pattern for Edit) ---

        // GET: /Campaigns/Edit/5 - Loads the campaign data into the 'Edit Campaign' form
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["Message"] = "Campaign ID is required for editing.";
                return RedirectToAction("Index", new { actionType = "view" });
            }

            var campaign = _context.Campaign.FirstOrDefault(c => c.CampaignID == id);
            if (campaign == null)
            {
                TempData["Message"] = "Campaign not found.";
                return RedirectToAction("Index", new { actionType = "view" });
            }

            ViewBag.ActionType = "edit";
            ViewBag.Campaigns = _context.Campaign.ToList();

            return View("ManageCampaign", campaign);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Campaign campaign)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ActionType = "edit";
                ViewBag.Campaigns = _context.Campaign.ToList();
                return View("ManageCampaign", campaign);
            }

            var campaignToUpdate = _context.Campaign.Find(campaign.CampaignID);
            if (campaignToUpdate == null)
            {
                TempData["Message"] = "Campaign not found.";
                return RedirectToAction("Index", new { actionType = "view" });
            }

            // Update fields
            _context.Entry(campaignToUpdate).CurrentValues.SetValues(campaign);
            _context.SaveChanges();

            TempData["Message"] = $"Campaign '{campaign.CampaignName}' updated successfully.";
            return RedirectToAction("Index", new { actionType = "view" });
        }

        // --- DELETE OPERATIONS ---

        // GET: /Campaigns/Delete/5 - Loads the campaign data for confirmation on the 'Delete Campaign' form
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["Message"] = "Campaign ID is required for deletion.";
                return RedirectToAction("Index", new { actionType = "view" });
            }

            var campaign = _context.Campaign.FirstOrDefault(c => c.CampaignID == id);
            if (campaign == null)
            {
                TempData["Message"] = "Campaign not found.";
                return RedirectToAction("Index", new { actionType = "view" });
            }

            ViewBag.ActionType = "delete";
            ViewBag.Campaigns = _context.Campaign.ToList();

            return View("ManageCampaign", campaign);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int campaignId) // Renamed from DeleteEntry
        {
            var campaign = _context.Campaign.FirstOrDefault(c => c.CampaignID == campaignId);
            if (campaign == null)
            {
                TempData["Message"] = "Campaign not found.";
                return RedirectToAction("Index", new { actionType = "view" });
            }

            _context.Campaign.Remove(campaign);
            _context.SaveChanges();

            TempData["Message"] = $"Campaign '{campaign.CampaignName}' deleted successfully.";
            return RedirectToAction("Index", new { actionType = "view" }); // Redirect to the main view
        }

        //--- Helper Method for Status Update ---

        // Moved the campaign update logic into a separate method for clarity and reuse
        private void UpdateCampaignStatusesAndImpressions()
        {
            var today = DateTime.Now;
            var campaignsToUpdate = _context.Campaign.ToList();

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
                else if (campaign.Status == "Active")
                    campaign.Impressions = new Random().Next(1000, 1500);
                else if (campaign.Status == "Completed")
                    campaign.Impressions = new Random().Next(1200, 1800);

            }

            // Save changes to the database
            _context.SaveChanges();
        }









        public IActionResult TrackCampaigns()
        {
            ViewBag.Section = "";
            ViewBag.Campaigns = new List<Campaign>();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TrackCampaigns(string section, string status, string type)
        {
            var today = DateTime.Today;
            var updatedCampaigns = new List<string>();

            // Step 1: Auto-update campaign statuses and impressions
            var campaignsToUpdate = _context.Campaign.ToList();

            foreach (var campaign in campaignsToUpdate)
            {
                var previousStatus = campaign.Status;
                var newStatus = previousStatus;

                if (campaign.StartDate <= today && campaign.EndDate >= today)
                    newStatus = "Active";
                else if (campaign.EndDate < today)
                    newStatus = "Completed";
                else if (campaign.StartDate > today)
                    newStatus = "Scheduled";

                if (newStatus != previousStatus)
                {
                    campaign.Status = newStatus;
                    updatedCampaigns.Add(campaign.CampaignName);
                }

                // Assign impressions based on status
                if (newStatus == "Scheduled")
                    campaign.Impressions = 0;
                else if (newStatus == "Active")
                    campaign.Impressions = new Random().Next(1000, 1500);
                else if (newStatus == "Completed")
                    campaign.Impressions = new Random().Next(1200, 1800);
            }

            _context.SaveChanges();

            // Step 2: Apply filters based on section/status/type
            var campaigns = _context.Campaign.AsQueryable();

            switch (section)
            {
                case "today":
                    campaigns = campaigns.Where(c => c.StartDate <= today && c.EndDate >= today && c.Status == "Active");
                    break;
                case "upcoming":
                    campaigns = campaigns.Where(c => c.StartDate > today && c.Status == "Scheduled");
                    break;
                case "completed":
                    campaigns = campaigns.Where(c => c.EndDate < today && c.Status == "Completed");
                    break;
                case "filter":
                    if (!string.IsNullOrEmpty(status))
                        campaigns = campaigns.Where(c => c.Status == status);
                    if (!string.IsNullOrEmpty(type))
                        campaigns = campaigns.Where(c => c.Type == type);
                    break;
            }

            ViewBag.Section = section;
            ViewBag.Campaigns = campaigns.ToList();
            ViewBag.UpdatedCampaigns = updatedCampaigns;

            return View();
        }





        public IActionResult Budget(string selectedType = null, int selectedCampaignId = 0, DateTime? selectedDate = null)
        {
            // Get all campaigns
            var campaigns = _context.Campaign.ToList();

            // Group by type and calculate total budget per type
            var groupedBudgets = campaigns
                .Where(c => !string.IsNullOrEmpty(c.Type))
                .GroupBy(c => c.Type)
                .Select(g => new
                {
                    Type = g.Key,
                    TotalBudget = g.Sum(c => c.Budget)
                })
                .ToList();

            // Filter campaigns by selected type
            var campaignsInType = string.IsNullOrEmpty(selectedType)
                ? new List<Campaign>()
                : campaigns.Where(c => c.Type == selectedType).ToList();

            // Select campaign by ID
            var selectedCampaign = selectedCampaignId > 0
                ? campaigns.FirstOrDefault(c => c.CampaignID == selectedCampaignId)
                : null;

            // Pass data to view using ViewBag
            ViewBag.CampaignTypes = groupedBudgets;
            ViewBag.CampaignsInType = campaignsInType;
            ViewBag.SelectedType = selectedType;
            ViewBag.SelectedCampaign = selectedCampaign;
            ViewBag.SelectedDate = selectedDate;

            return View();
        }














        public async Task<IActionResult> CampaignPerformance(string selectedType)
        {
            // Load campaign types for dropdown
            var campaignTypes = await _context.Campaign
                .Select(c => c.Type)
                .Distinct()
                .ToListAsync();

            ViewBag.CampaignTypes = campaignTypes;
            ViewBag.SelectedType = selectedType;

            if (string.IsNullOrEmpty(selectedType))
                return View(new List<dynamic>());

            // Get campaigns under selected type
            var campaigns = await _context.Campaign
                .Where(c => c.Type == selectedType)
                .Select(c => new
                {
                    c.CampaignName,
                    c.Impressions,
                    TotalResponses = c.CustomerResponse.Count(),
                    AverageRating = c.CustomerResponse.Average(r => (double?)r.Rating)

                })
                .ToListAsync();

            // Identify top campaign by response count
            var topCampaign = campaigns
                .OrderByDescending(c => c.TotalResponses)
                .FirstOrDefault();

            ViewBag.TopCampaign = topCampaign;

            return View(campaigns);
        }


    }
}
