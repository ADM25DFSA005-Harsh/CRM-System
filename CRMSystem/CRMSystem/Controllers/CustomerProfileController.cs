using Microsoft.AspNetCore.Mvc;

using CRM.Models;

using CRMSystem.Data;

using System.Collections.Generic;

using System.Linq;

namespace CRM.Controllers

{

    [ApiController]

    [Route("api/[controller]")]

    public class CustomerProfileController : ControllerBase

    {

        private readonly CrmDbContext _db;

        public CustomerProfileController(CrmDbContext db)

        {

            _db = db;

        }

        // GET: api/CustomerProfile?showRecords=true&searchTerm=abc

        [HttpGet]

        public IActionResult GetProfiles([FromQuery] bool showRecords = false, [FromQuery] string searchTerm = "")

        {

            var profiles = _db.CustomerProfile.AsQueryable();

            if (showRecords && !string.IsNullOrWhiteSpace(searchTerm))

            {

                string term = searchTerm.ToLower();

                profiles = profiles.Where(p =>

                    (!string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(term)) ||

                    (!string.IsNullOrEmpty(p.Email) && p.Email.ToLower().Contains(term)) ||

                    (!string.IsNullOrEmpty(p.Region) && p.Region.ToLower().Contains(term))

                );

            }

            return Ok(profiles.ToList());

        }

        // GET: api/CustomerProfile/{id}

        [HttpGet("{id}")]

        public IActionResult GetDetails(int id)

        {

            var customer = _db.CustomerProfile.Find(id);

            if (customer == null)

                return NotFound("Customer not found.");

            return Ok(customer);

        }

        // GET: api/CustomerProfile/check-email?email=abc@example.com

        [HttpGet("check-email")]

        public IActionResult CheckEmailExists([FromQuery] string email)

        {

            if (string.IsNullOrWhiteSpace(email))

                return BadRequest("Email is required.");

            bool exists = _db.CustomerProfile.Any(p => p.Email.ToLower() == email.ToLower());

            return Ok(exists);

        }

        // GET: api/CustomerProfile/check-phone?number=1234567890

        [HttpGet("check-phone")]

        public IActionResult CheckPhoneExists([FromQuery] string number)

        {

            if (string.IsNullOrWhiteSpace(number))

                return BadRequest("Phone number is required.");

            bool exists = _db.CustomerProfile.Any(p => p.PhoneNumber == number);

            return Ok(exists);

        }

        // POST: api/CustomerProfile

        [HttpPost]

        public IActionResult CreateProfile([FromBody] CustomerProfile model)

        {

            if (!ModelState.IsValid)

                return BadRequest(ModelState);

            bool emailExists = _db.CustomerProfile.Any(p => p.Email.ToLower() == model.Email.ToLower());

            if (emailExists)

                return Conflict("Email already exists.");

            bool phoneExists = _db.CustomerProfile.Any(p => p.PhoneNumber == model.PhoneNumber);

            if (phoneExists)

                return Conflict("Phone number already exists.");

            _db.CustomerProfile.Add(model);

            _db.SaveChanges();

            return CreatedAtAction(nameof(GetDetails), new { id = model.CustomerID }, model);

        }

        // PUT: api/CustomerProfile/{id}

        [HttpPut("{id}")]

        public IActionResult UpdateProfile(int id, [FromBody] CustomerProfile model)

        {

            if (id != model.CustomerID)

                return BadRequest("ID mismatch.");

            if (!ModelState.IsValid)

                return BadRequest(ModelState);

            var existing = _db.CustomerProfile.Find(id);

            if (existing == null)

                return NotFound("Customer not found.");

            bool emailTaken = _db.CustomerProfile.Any(p => p.Email.ToLower() == model.Email.ToLower() && p.CustomerID != id);

            if (emailTaken)

                return Conflict("Email already exists for another customer.");

            bool phoneTaken = _db.CustomerProfile.Any(p => p.PhoneNumber == model.PhoneNumber && p.CustomerID != id);

            if (phoneTaken)

                return Conflict("Phone number already exists for another customer.");

            _db.Entry(existing).CurrentValues.SetValues(model);

            _db.SaveChanges();

            return Ok(new { message = "Customer updated successfully." });

        }

        // DELETE: api/CustomerProfile/{id}

        [HttpDelete("{id}")]

        public IActionResult DeleteProfile(int id)

        {

            var customer = _db.CustomerProfile.Find(id);

            if (customer == null)

                return NotFound("Customer not found.");

            _db.CustomerProfile.Remove(customer);

            _db.SaveChanges();

            return Ok("Customer deleted successfully.");

        }

    }

}

