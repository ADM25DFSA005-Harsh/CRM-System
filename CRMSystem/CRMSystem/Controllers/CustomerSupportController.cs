using CRMSystem.Data;
using Microsoft.AspNetCore.Mvc;
using CRMSystem.Models;
using CRMProject.Models;

namespace CRMProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerSupportController : Controller
    {
        CrmDbContext supportTicketDb;

        public CustomerSupportController(CrmDbContext _supportTicketDb)
        {
            supportTicketDb = _supportTicketDb;

        }

        // POST: Create a new ticket
        [HttpPost("create")]
        public IActionResult CreateTicket([FromBody] SupportTicket data)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            supportTicketDb.SupportTicket.Add(data);
            supportTicketDb.SaveChanges();

            return Ok(data);
        }

        // GET: Get all tickets
        [HttpGet("all")]
        public IActionResult GetAllTickets()
        {
            var tickets = supportTicketDb.SupportTicket.ToList();
            return Ok(tickets);
        }

        // GET: Filter tickets by status (e.g., "Pending")
        [HttpGet("status")]
        public IActionResult GetTicketsByStatus([FromQuery] string status)
        {
            var tickets = supportTicketDb.SupportTicket
                .Where(t => t.Status != null && t.Status.ToLower() == status.ToLower())
                .ToList();

            return Ok(tickets);
        }

        // GET: Filter tickets by assigned agent name
        [HttpGet("agent")]
        public IActionResult GetTicketsByAgent([FromQuery] string agentName)
        {
            var tickets = supportTicketDb.SupportTicket
                .Where(t => t.AssignedAgent != null && t.AssignedAgent.ToLower() == agentName.ToLower())
                .ToList();

            return Ok(tickets);
        }

        // GET: View ticket by TicketID
        [HttpGet("view/{ticketId}")]
        public IActionResult ViewTicketById(int ticketId)
        {
            var ticket = supportTicketDb.SupportTicket.FirstOrDefault(t => t.TicketID == ticketId);
            if (ticket == null)
                return NotFound($"Ticket with ID {ticketId} not found.");

            return Ok(ticket);
        }

        // PUT: Edit ticket details
        [HttpPut("edit/{ticketId}")]
        public IActionResult EditTicket(int ticketId, [FromBody] SupportTicket updatedTicket)
        {
            var existingTicket = supportTicketDb.SupportTicket.FirstOrDefault(t => t.TicketID == ticketId);
            if (existingTicket == null)
                return NotFound($"Ticket with ID {ticketId} not found.");

            existingTicket.Issue = updatedTicket.Issue;
            existingTicket.IssueDescription = updatedTicket.IssueDescription;
            existingTicket.AssignedAgent = updatedTicket.AssignedAgent;
            existingTicket.Status = updatedTicket.Status;

            supportTicketDb.SaveChanges();
            return Ok(existingTicket);
        }
    }
}
