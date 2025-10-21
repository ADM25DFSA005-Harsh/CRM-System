//using CRMSystem.Data;
//using Microsoft.AspNetCore.Mvc;
//using CRMSystem.Models;
//using CRMProject.Models;

//namespace CRMProject.Controllers
//{
//    public class CustomerSupportController : Controller
//    {
//        CrmDbContext supportTicketDb;

//        public CustomerSupportController(CrmDbContext _supportTicketDb)
//        {
//            supportTicketDb = _supportTicketDb;

//        }

//        [HttpGet]
//        public IActionResult DisplayTicketsInput()
//        {
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult DisplayTicketsInput(SupportTicket data)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(data);
//            }

//            supportTicketDb.SupportTicket.Add(data);
//            supportTicketDb.SaveChanges();

//            return RedirectToAction("DisplayInTableForm");
//        }

//        [HttpGet]
//        public IActionResult DisplayInTableForm(string statusFilter)
//        {
//            var tickets = string.IsNullOrEmpty(statusFilter)
//                ? supportTicketDb.SupportTicket.ToList()
//                : supportTicketDb.SupportTicket.Where(t => t.Status == statusFilter).ToList();

//            return View(tickets);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult DeleteByCustomerId(int customerId)
//        {
//            var ticket = supportTicketDb.SupportTicket.FirstOrDefault(t => t.CustomerID == customerId);
//            if (ticket != null)
//            {
//                supportTicketDb.SupportTicket.Remove(ticket);
//                supportTicketDb.SaveChanges();
//            }
//            return RedirectToAction("DisplayInTableForm");
//        }

//        [HttpGet]
//        public IActionResult EditByCustomerId(int customerId)
//        {
//            var ticket = supportTicketDb.SupportTicket.FirstOrDefault(t => t.CustomerID == customerId);
//            if (ticket == null)
//            {
//                return NotFound();
//            }
//            return View(ticket);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult EditByCustomerId(SupportTicket updatedTicket)
//        {
//            var existingTicket = supportTicketDb.SupportTicket.FirstOrDefault(t => t.CustomerID == updatedTicket.CustomerID);
//            if (existingTicket == null)
//            {
//                return NotFound();
//            }

//            existingTicket.Issue = updatedTicket.Issue;
//            existingTicket.AssignedAgent = updatedTicket.AssignedAgent;
//            existingTicket.Status = updatedTicket.Status;

//            supportTicketDb.SaveChanges();
//            return RedirectToAction("DisplayInTableForm");
//        }

//        [HttpGet]
//        public IActionResult ViewTicket(int customerId)
//        {
//            var ticket = supportTicketDb.SupportTicket.FirstOrDefault(t => t.CustomerID == customerId);
//            if (ticket == null)
//            {
//                return NotFound();
//            }
//            return View(ticket);
//        }

//        [HttpGet]
//        public IActionResult DisplayInTableForm(int? SearchById, string? statusFilter)
//        {
//            var tickets = supportTicketDb.SupportTicket.AsQueryable();

//            if (SearchById.HasValue)
//            {
//                tickets = tickets.Where(t => t.CustomerID == SearchById.Value);
//            }

//            if (!string.IsNullOrEmpty(statusFilter))
//            {
//                tickets = tickets.Where(t => t.Status == statusFilter);
//            }

//            return View(tickets.ToList());
//        }



//    }
//}

using CRMSystem.Data;
using Microsoft.AspNetCore.Mvc;
using CRMSystem.Models;
using CRMProject.Models;

namespace CRMProject.Controllers
{
    public class CustomerSupportController : Controller
    {
        private readonly CrmDbContext supportTicketDb;

        public CustomerSupportController(CrmDbContext _supportTicketDb)
        {
            supportTicketDb = _supportTicketDb;
        }

        // GET: Ticket input form
        [HttpGet]
        public IActionResult DisplayTicketsInput()
        {
            return View();
        }

        // POST: Save new ticket
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

        // GET: Display filtered tickets (by CustomerID or Status or both)
        [HttpGet]
        public IActionResult DisplayInTableForm(int? SearchById, string? statusFilter)
        {
            var tickets = supportTicketDb.SupportTicket.AsQueryable();

            if (SearchById.HasValue)
            {
                tickets = tickets.Where(t => t.CustomerID == SearchById.Value);
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                tickets = tickets.Where(t => t.Status == statusFilter);
            }

            return View(tickets.ToList());
        }

        // POST: Delete ticket by CustomerID
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

        // GET: Edit ticket by CustomerID
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

        // POST: Save edited ticket
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
            existingTicket.IssueDescription = updatedTicket.IssueDescription;
            existingTicket.AssignedAgent = updatedTicket.AssignedAgent;
            existingTicket.Status = updatedTicket.Status;

            supportTicketDb.SaveChanges();
            return RedirectToAction("DisplayInTableForm");
        }

        // GET: View ticket details by CustomerID
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





// ﻿using CRMSystem.Data;
// using Microsoft.AspNetCore.Mvc;
// using CRMSystem.Models;
// using CRMProject.Models;

// namespace CRMProject.Controllers
// {
//     public class CustomerSupportController : Controller
//     {
//         CrmDbContext supportTicketDb;

//         public CustomerSupportController(CrmDbContext _supportTicketDb)
//         {
//             supportTicketDb = _supportTicketDb;

//         }

//         [HttpGet]
//         public IActionResult DisplayTicketsInput()
//         {
//             return View();
//         }

//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public IActionResult DisplayTicketsInput(SupportTicket data)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return View(data);
//             }

//             supportTicketDb.SupportTicket.Add(data);
//             supportTicketDb.SaveChanges();

//             return RedirectToAction("DisplayInTableForm");
//         }

//         [HttpGet]
//         public IActionResult DisplayInTableForm(string statusFilter)
//         {
//             var tickets = string.IsNullOrEmpty(statusFilter)
//                 ? supportTicketDb.SupportTicket.ToList()
//                 : supportTicketDb.SupportTicket.Where(t => t.Status == statusFilter).ToList();

//             return View(tickets);
//         }
        
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public IActionResult DeleteByCustomerId(int customerId)
//         {
//             var ticket = supportTicketDb.SupportTicket.FirstOrDefault(t => t.CustomerID == customerId);
//             if (ticket != null)
//             {
//                 supportTicketDb.SupportTicket.Remove(ticket);
//                 supportTicketDb.SaveChanges();
//             }
//             return RedirectToAction("DisplayInTableForm");
//         }

//         [HttpGet]
//         public IActionResult EditByCustomerId(int customerId)
//         {
//             var ticket = supportTicketDb.SupportTicket.FirstOrDefault(t => t.CustomerID == customerId);
//             if (ticket == null)
//             {
//                 return NotFound();
//             }
//             return View(ticket);
//         }

//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public IActionResult EditByCustomerId(SupportTicket updatedTicket)
//         {
//             var existingTicket = supportTicketDb.SupportTicket.FirstOrDefault(t => t.CustomerID == updatedTicket.CustomerID);
//             if (existingTicket == null)
//             {
//                 return NotFound();
//             }

//             existingTicket.Issue = updatedTicket.Issue;
//             existingTicket.AssignedAgent = updatedTicket.AssignedAgent;
//             existingTicket.Status = updatedTicket.Status;

//             supportTicketDb.SaveChanges();
//             return RedirectToAction("DisplayInTableForm");
//         }

//         [HttpGet]
//         public IActionResult ViewTicket(int customerId)
//         {
//             var ticket = supportTicketDb.SupportTicket.FirstOrDefault(t => t.CustomerID == customerId);
//             if (ticket == null)
//             {
//                 return NotFound();
//             }
//             return View(ticket);
//         }

//     }
// }
