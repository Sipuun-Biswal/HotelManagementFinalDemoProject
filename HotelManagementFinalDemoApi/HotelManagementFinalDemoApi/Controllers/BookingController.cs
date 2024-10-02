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
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllBooking")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings()
        {

            var today = DateTime.Now.Date;

           
            var pastBookings = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.CheckOutDate < today) 
                .ToListAsync();

            foreach (var booking in pastBookings)
            {
                var room = booking.Room;
                room.IsAvailable = true; 
                _context.Entry(room).State = EntityState.Modified;
                _context.Bookings.Remove(booking); 
            }
            await _context.SaveChangesAsync();
            var bookingsForToday = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.CheckInDate <= today && b.CheckOutDate > today) 
                .ToListAsync();

            foreach (var booking in bookingsForToday)
            {
                var room = booking.Room;
                room.IsAvailable = false; 
                _context.Entry(room).State = EntityState.Modified;
            }

            var availableRooms = await _context.Rooms
                .Where(r => !_context.Bookings.Any(b => b.RoomId == r.Id
                        && b.CheckInDate <= today
                        && b.CheckOutDate > today))  
                .ToListAsync();

            foreach (var room in availableRooms)
            {
                room.IsAvailable = true;  
                _context.Entry(room).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            var currentBookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .ThenInclude(b => b.Hotel)
                .ToListAsync();

            var bookingDtos = currentBookings.Select(b => BookingDto.FromEntity(b)).ToList();

            return Ok(bookingDtos);
        }

        [HttpPost("createBooking")]
        [Authorize(Roles ="Admin,Manager,User")]
        public async Task<ActionResult<BookingDto>> PostBooking(BookingDto bookingDto)
        {
            if (bookingDto.CheckInDate.Date < DateTime.Now.Date || bookingDto.CheckInDate>bookingDto.CheckOutDate)
            {
                return BadRequest("The check-in date cannot be in the past. and Check-In date should be smaller than check-Out date.");
            }
            bool isRoomBooked = await _context.Bookings
                .AnyAsync(b => b.RoomId == bookingDto.RoomId
                            && b.CheckInDate < bookingDto.CheckOutDate
                            && b.CheckOutDate > bookingDto.CheckInDate);

            if (isRoomBooked)
            {
                return BadRequest("The room is already booked for the selected dates.");
            }
            var room = await _context.Rooms.FindAsync(bookingDto.RoomId);
            if (room == null || !room.IsAvailable)
            {
                return BadRequest("The room is not available.");
            }
            var booking = BookingDto.ToEntity(bookingDto);
            booking.Id = Guid.NewGuid();
            booking.CreatedDate = DateTime.Now;
            booking.Status = 1;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookingsByUser", new { userId = booking.UserId }, BookingDto.FromEntity(booking));
        }
        [HttpGet("GetBookingById/{id}")]
        [Authorize(Roles ="Admin,Manager,User")]
        public async Task<ActionResult<BookingDto>> GetBookingById(Guid id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .ThenInclude(b => b.Hotel)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            var bookingDto = BookingDto.FromEntity(booking);
            return Ok(bookingDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles ="Admin,Manager,User")]
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
        [Authorize(Roles ="Admin,Manager,User")]
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
        [Authorize(Roles ="Admin,Manager,User")]
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
        //For Manager
        [HttpGet("GetBookingsByHotel/{hotelId}")]
        [Authorize(Roles ="Manager")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByHotel(Guid hotelId)
        {
            var hotelBookings = await _context.Bookings
                .Include(b => b.User)    
                .Include(b => b.Room)    
                .ThenInclude(r => r.Hotel) 
                .Where(b => b.Room.HotelId == hotelId) 
                .ToListAsync();
            var bookingDtos = hotelBookings.Select(BookingDto.FromEntity).ToList();
            return Ok(bookingDtos);
        }


    }
}
