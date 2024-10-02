using HotelManagementFinalDemoApi.Models.DataBaseDto;
using HotelManagementFinalDemoApi.Models.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementFinalDemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public HotelController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("All")]
        [Authorize(Roles ="Admin,Manager,User")]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetAllHotels()
        {
            var hotels = await _context.Hotels.Where(u=>u.IsActive).
                                                Include(h => h.User)
                                              .Include(h => h.Country)
                                              .Include(h => h.State)
                                              .Include(h => h.City)
                                              .ToListAsync();
            var hotelDtos = HotelDto.FromEntity(hotels);
            return Ok(hotelDtos);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<ActionResult<HotelDto>> GetHotelById(Guid id)
        {
            var hotel = await _context.Hotels.Where(h=>h.Id==id && h.IsActive)
                                              .Include(h => h.User)
                                             .Include(h => h.Country)
                                             .Include(h => h.State)
                                             .Include(h => h.City)
                                             .FirstOrDefaultAsync();
            if (hotel == null)
            {
                return NotFound("Hotel not found.");
            }

            var hotelDto = HotelDto.FromEntity(hotel);
            return Ok(hotelDto);
        }
        [HttpPost]
        [Authorize(Roles="Admin")]
        public async Task<ActionResult<HotelDto>> CreateHotel([FromBody] HotelDto hotelDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hotel = HotelDto.ToEntity(hotelDto);
            hotel.Id = Guid.NewGuid();

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHotelById), new { id = hotel.Id }, HotelDto.FromEntity(hotel));
        }

        [HttpPut("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] HotelDto hotelDto)
    {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != hotelDto.Id)
            {
                return BadRequest("Hotel ID mismatch.");
            }

            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound("Hotel not found.");
            }

            // Update hotel properties
            hotel.HotelName = hotelDto.HotelName;
            hotel.CountryId = hotelDto.CountryId;
            hotel.StateId = hotelDto.StateId;
            hotel.CityId = hotelDto.CityId;
            hotel.UserId = hotelDto.UserId;
            hotel.PhoneNo = hotelDto.PhoneNo;
            hotel.Address = hotelDto.Address;
            hotel.HotelImage = hotelDto.HotelImage;

            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHotel(Guid id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return NotFound("Hotel not found.");
            }
            hotel.IsActive = false;

            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("ByUser/{userId}")]
        public async Task<ActionResult<HotelDto>> GetHotelByUserId(Guid userId)
        {
            // Fetch the hotel associated with the provided userId and ensure it's active
            var hotel = await _context.Hotels
                .Where(h => h.UserId == userId && h.IsActive)
                .Include(h => h.User)
                .Include(h => h.Country)
                .Include(h => h.State)
                .Include(h => h.City)
                .FirstOrDefaultAsync();

            if (hotel == null)
            {
                return NotFound("No active hotel found for the specified user.");
            }

            var hotelDto = HotelDto.FromEntity(hotel);
            return Ok(hotelDto);
        }
    }
}
