using Microsoft.AspNetCore.Authorization;
using ContactAPI.Data;
using ContactAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Azure.Identity;

namespace ContactAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : Controller
    {
        private readonly ContactAPIDbContext dbContext;

        private readonly IJwtAuthenticationManager _jwtAuthenticationManager;

        public ContactController(IJwtAuthenticationManager jwtAuthenticationManger, ContactAPIDbContext _dbContext)
        {
            _jwtAuthenticationManager = jwtAuthenticationManger;
            dbContext = _dbContext;
        }

        //public ContactController(ContactAPIDbContext dbContext)
        //{
        //    this.dbContext = dbContext;
        //}
        // Created Get Request
        [HttpGet]
        //public IActionResult GetContact()
        //{
        //    return Ok(dbContext.Contacts.ToList());
        //}

        // GET: api/<NameController>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(dbContext.Contacts.ToList());
        }

        // GET api/<NameController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet]
        [Route("{Id:guid}")]

        public IActionResult GetaContact([FromRoute] Guid Id)
        {
            var contact = dbContext.Contacts.Find(Id);
            if (contact == null)
            {
                return NotFound();
            }
            return Ok(contact);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserCred userCred)
        {
            var token = _jwtAuthenticationManager.Authenticate
                (userCred.Username, userCred.Password);
            if (token == null)
                return Unauthorized();
            return Ok(token);

            var users = new UserCred()
            {
                Username = userCred.Username,
                Password = userCred.Password
            };
            dbContext.Users.Add(users);
            dbContext.SaveChanges();
            return Ok(users);
        }

        // Created Post Request 
        [HttpPost]
        public async Task<IActionResult> PostContact(AddContact addContact)
        {
            var contact = new Contact()
            {
                Id = Guid.NewGuid(),
                Name = addContact.Name,
                Email = addContact.Email,
                Phone = addContact.Phone,
                Address = addContact.Address,
            };
            await dbContext.Contacts.AddAsync(contact);
            await dbContext.SaveChangesAsync();
            return Ok(contact);
        }

        [HttpPut]
        [Route("{Id:guid}")]
        public async Task<IActionResult> UpdateContacts([FromRoute] Guid Id, UpdateContact updateContact)
        {
            var contact = dbContext.Contacts.Find(Id);

            if (contact != null)
            {
                contact.Name = updateContact.Name;
                contact.Email = updateContact.Email;
                contact.Phone = updateContact.Phone;
                contact.Address = updateContact.Address;
                await dbContext.SaveChangesAsync();
                return Ok(contact);
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{Id:Guid}")]

        public async Task<IActionResult> DeleteContact([FromRoute] Guid Id)
        {
            var contact = await dbContext.Contacts.FindAsync(Id);
            if(contact != null)
            {
                dbContext.Remove(contact);
                await dbContext.SaveChangesAsync();
                return Ok(contact);
            }
            return NotFound();
        }
    } 
}
