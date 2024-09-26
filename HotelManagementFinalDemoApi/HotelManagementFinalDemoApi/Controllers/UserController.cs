using HotelManagementFinalDemoApi.Models.DataBaseDto;
using HotelManagementFinalDemoApi.Helpers;
using HotelManagementFinalDemoApi.Models.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace HotelManagementFinalDemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.Where(u=>u.IsActive).ToListAsync();
            var userDtos = UserDto.FromEntity(users);
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {

            var user = await _context.Users.Where(u => u.Id == id && u.IsActive).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            var userDto = UserDto.FromEntity(user);
            return Ok(userDto);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (existingUser != null)
            {
                return BadRequest("Email is already registered.");
            }


            if (string.IsNullOrWhiteSpace(userDto.ProfileImage))
            {
                userDto.ProfileImage = null;
            }
            var user = UserDto.ToEntity(userDto);
            user.Id=Guid.NewGuid();

            user.PasswordHash = PasswordHash.HashPassword(userDto.Password); // Hash the password

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, UserDto.FromEntity(user));

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userDto.Id)
            {
                return BadRequest("User ID mismatch.");
            }


            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }


            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Email = userDto.Email;
            user.ProfileImage = userDto.ProfileImage;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsers(Guid id)
        {
             var user= await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            user.IsActive = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}
