using HotelManagementFinalDemoApi.Models.DataBaseDto;
using HotelManagementFinalDemoApi.Models.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementFinalDemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllBooking")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings()
        {

            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .ThenInclude(b=>b.Hotel)
                .ToListAsync();

            var bookingDtos = bookings.Select(b => BookingDto.FromEntity(b)).ToList();
            return Ok(bookingDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(Guid id)
        {

            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return BookingDto.FromEntity(booking);
        }

        [HttpPost("createBooking")]
        public async Task<ActionResult<BookingDto>> PostBooking(BookingDto bookingDto)
        {
            if (bookingDto.CheckInDate.Date < DateTime.Now.Date)
            {
                return BadRequest("The check-in date cannot be in the past. Please select a date from today or later.");
            }
            bool isRoomBooked = await _context.Bookings
                .AnyAsync(b => b.RoomId == bookingDto.RoomId
                            && b.CheckInDate < bookingDto.CheckOutDate
                            && b.CheckOutDate > bookingDto.CheckInDate);

            if (isRoomBooked)
            {
                return BadRequest("The room is already booked for the selected dates.");
            }

            var booking = BookingDto.ToEntity(bookingDto);
            booking.Id = Guid.NewGuid();
            booking.CreatedDate = DateTime.Now;
            booking.Status = 1;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookingsByUser", new { userId = booking.UserId }, BookingDto.FromEntity(booking));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(Guid id, BookingDto bookingDto)
        {
            if (id != bookingDto.Id)
            {
                return BadRequest();
            }

           
            bool isRoomBooked = await _context.Bookings
                .AnyAsync(b => b.RoomId == bookingDto.RoomId
                            && b.Id != id
                            && b.CheckInDate < bookingDto.CheckOutDate
                            && b.CheckOutDate > bookingDto.CheckInDate);

            if (isRoomBooked)
            {
                return BadRequest("The room is already booked for the selected dates.");
            }

            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            // Update the booking
            booking.UserId = bookingDto.UserId;
            booking.RoomId = bookingDto.RoomId;
            booking.CheckInDate = bookingDto.CheckInDate;
            booking.CheckOutDate = bookingDto.CheckOutDate;
            booking.Status = (int)bookingDto.Status;
            booking.UpdatedBy = bookingDto.UpdatedBy;
            booking.UpdatedAt = DateTime.Now;

            _context.Entry(booking).State = EntityState.Modified;

            if (!await BookingExists(id))
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> BookingExists(Guid id)
        {
            return await _context.Bookings.AnyAsync(e => e.Id == id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);  
             _context.SaveChanges();

            return NoContent();
        }

        [HttpGet("GetBookingsByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByUser(Guid userId)
        {
            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .ThenInclude(b => b.Hotel)
                .Where(b => b.UserId == userId);

             var userBookings = await query.ToListAsync();

            var bookingDtos = userBookings.Select(b => BookingDto.FromEntity(b)).ToList();
            return Ok(bookingDtos);
        }


    }
}
