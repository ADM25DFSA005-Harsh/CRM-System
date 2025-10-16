using Microsoft.AspNetCore.Mvc;
using CRM.Models;
using CRMSystem.Data;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Controllers
{
    public class CustomerProfileController : Controller
    {
        private readonly CrmDbContext _db;

        public CustomerProfileController(CrmDbContext db)
        {
            _db = db;
        }

        // GET: Display customer profiles with optional search
        [HttpGet]
        public IActionResult CustomerProfile(bool showRecords = false, string searchTerm = "")
        {
            List<CustomerProfile> profiles = new();

            if (showRecords)
            {
                
                profiles = _db.customerProfiles.Select(a => a).ToList();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    string term = searchTerm.ToLower();
                    profiles = profiles.Where(p =>
                        (!string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(term)) ||
                        (!string.IsNullOrEmpty(p.Email) && p.Email.ToLower().Contains(term)) ||
                        (!string.IsNullOrEmpty(p.Region) && p.Region.ToLower().Contains(term))
                    ).ToList();
                }
            }

            ViewBag.ShowRecords = showRecords;
            return View("DisplayProfiles", profiles);
        }

        // GET: Show form to add new customer
        [HttpGet]
        public IActionResult TakeInput()
        {
            return View();
        }

        // POST: Save new customer and redirect to details
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TakeInput(CustomerProfile model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _db.customerProfiles.Add(model);
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Customer added successfully!";
                return RedirectToAction("Details", new { id = model.CustomerID });
            }
            catch
            {
                TempData["ErrorMessage"] = "Error adding customer.";
                return View(model);
            }
        }

        // GET: Customer details by ID
        [HttpGet]
        public IActionResult Details(int id)
        {
            var customer = _db.customerProfiles.Find(id);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction("CustomerProfile", new { showRecords = true });
            }

            return View(customer);
        }

        // GET: Edit customer by ID
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var customer = _db.customerProfiles.Find(id);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction("CustomerProfile", new { showRecords = true });
            }

            return View(customer);
        }

        // POST: Save edited customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CustomerProfile model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _db.customerProfiles.Update(model);
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Customer updated successfully!";
                return RedirectToAction("Details", new { id = model.CustomerID });
            }
            catch
            {
                TempData["ErrorMessage"] = "Error updating customer.";
                return View(model);
            }
        }

        // GET: Confirm deletion
        [HttpGet]
        public IActionResult DeleteData(int id)
        {
            var customer = _db.customerProfiles.Find(id);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction("CustomerProfile", new { showRecords = true });
            }

            return View(customer);
        }

        // POST: Perform deletion and show confirmation
        [HttpPost, ActionName("DeleteData")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDelete(int id)
        {
            var customer = _db.customerProfiles.Find(id);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction("CustomerProfile", new { showRecords = true });
            }

            try
            {
                _db.customerProfiles.Remove(customer);
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Customer deleted successfully!";
                return RedirectToAction("DeleteConfirmation");
            }
            catch
            {
                TempData["ErrorMessage"] = "Error deleting customer.";
                return RedirectToAction("CustomerProfile", new { showRecords = true });
            }
        }

        // GET: Show delete confirmation message
        [HttpGet]
        public IActionResult DeleteConfirmation()
        {
            return View();
        }
    }
}



