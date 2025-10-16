using CRMSystem.Data;
using Microsoft.AspNetCore.Mvc;
using CRMSystem.Models;
using CRMProject.Models;

namespace CRMProject.Controllers
{
    public class CustomerSupportController : Controller
    {
        CrmDbContext supportTicketDb;

        public CustomerSupportController(CrmDbContext _supportTicketDb)
        {
            supportTicketDb = _supportTicketDb;

        }

        [HttpGet]
        public IActionResult DisplayTicketsInput()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DisplayTicketsInput(SupportTicket data)
        {
            if (!ModelState.IsValid)
            {
                return View(data);
            }

            supportTicketDb.SupportTicket.Add(data);
            supportTicketDb.SaveChanges();

            return RedirectToAction("DisplayInTableForm");
        }

        [HttpGet]
        public IActionResult DisplayInTableForm(string statusFilter)
        {
            var tickets = string.IsNullOrEmpty(statusFilter)
                ? supportTicketDb.SupportTicket.ToList()
                : supportTicketDb.SupportTicket.Where(t => t.Status == statusFilter).ToList();

            return View(tickets);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteByCustomerId(int customerId)
        {
            var ticket = supportTicketDb.SupportTicket.FirstOrDefault(t => t.CustomerID == customerId);
            if (ticket != null)
            {
                supportTicketDb.SupportTicket.Remove(ticket);
                supportTicketDb.SaveChanges();
            }
            return RedirectToAction("DisplayInTableForm");
        }

        [HttpGet]
        public IActionResult EditByCustomerId(int customerId)
        {
            var ticket = supportTicketDb.SupportTicket.FirstOrDefault(t => t.CustomerID == customerId);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditByCustomerId(SupportTicket updatedTicket)
        {
            var existingTicket = supportTicketDb.SupportTicket.FirstOrDefault(t => t.CustomerID == updatedTicket.CustomerID);
            if (existingTicket == null)
            {
                return NotFound();
            }

            existingTicket.Issue = updatedTicket.Issue;
            existingTicket.AssignedAgent = updatedTicket.AssignedAgent;
            existingTicket.Status = updatedTicket.Status;

            supportTicketDb.SaveChanges();
            return RedirectToAction("DisplayInTableForm");
        }

        [HttpGet]
        public IActionResult ViewTicket(int customerId)
        {
            var ticket = supportTicketDb.SupportTicket.FirstOrDefault(t => t.CustomerID == customerId);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

    }
}
