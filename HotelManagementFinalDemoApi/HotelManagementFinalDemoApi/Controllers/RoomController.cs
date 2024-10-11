using HotelManagementFinalDemoApi.Models.DataBaseDto;
using HotelManagementFinalDemoApi.Models.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementFinalDemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("AllRoom")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
        {
            var rooms = await _context.Rooms
                                      .Include(r => r.Hotel) 
                                      .ToListAsync();
            var roomDtos = rooms.Select(RoomDto.FromEntity).ToList();
            return Ok(roomDtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDto>> GetRoom(Guid id)
        {
            var room = await _context.Rooms
                                     .Include(r => r.Hotel) 
                                     .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var roomDto = RoomDto.FromEntity(room);
            return Ok(roomDto);
        }

        [HttpPost("CreateRoom")]
        public async Task<ActionResult<RoomDto>> PostRoom([FromBody]RoomDto roomDto)
        {
            bool isRoomNameExists = await _context.Rooms.AnyAsync(r => r.RoomNo == roomDto.RoomNo && r.HotelId == roomDto.HotelId);
            if (isRoomNameExists)
            {
                return BadRequest("A room with the same number already exists in this hotel.");
            }
            if (roomDto.RoomNo <= 0) // Assuming `NumberOfRooms` is a property of `RoomDto`
            {
                return BadRequest("Room No must be greater than zero .");
            }
            if (roomDto.Price <= 0) // Assuming `NumberOfRooms` is a property of `RoomDto`
            {
                return BadRequest(" Room price must be greater than zero .");
            }
            var room = RoomDto.ToEntity(roomDto);
            room.Id = Guid.NewGuid();
            room.IsAvailable=true;
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            roomDto.Id = room.Id; 
            return CreatedAtAction("GetRoom", new { id = roomDto.Id }, roomDto);
        }
        //Room created by manager.
        [HttpPost("CreateRoomByUserId")]
        public async Task<ActionResult<RoomDto>> CreateRoomByUserId([FromBody] RoomDto roomDto, [FromQuery] Guid userId)
        {
            if (roomDto.RoomNo <= 0) // Assuming `NumberOfRooms` is a property of `RoomDto`
            {
                return BadRequest("Room No must be greater than zero .");
            }
            if (roomDto.Price <= 0) // Assuming `NumberOfRooms` is a property of `RoomDto`
            {
                return BadRequest(" Room price must be greater than zero .");
            }
            // Check if the hotel associated with the room has the provided user as its owner
            var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == roomDto.HotelId && h.UserId == userId);

            if (hotel == null)
            {
                return Forbid("You are not authorized to create a room for this hotel.");
            }

            // Check if the room number already exists in the hotel
            bool isRoomExists = await _context.Rooms.AnyAsync(r => r.RoomNo == roomDto.RoomNo && r.HotelId == roomDto.HotelId);
            if (isRoomExists)
            {
                return BadRequest("A room with the same number already exists in this hotel.");
            }

            // Create and save the new room
            var room = RoomDto.ToEntity(roomDto);
            room.Id = Guid.NewGuid();
            room.IsAvailable = true;

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return Ok();
        }

        //Room allocated to manager.
        [HttpGet("GetRoomsByHotelUser/{userId}")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsByHotelUser(Guid userId)
        {
            var rooms = await _context.Rooms
                                      .Include(r => r.Hotel) 
                                      .Where(r => r.Hotel.UserId == userId)
                                      .ToListAsync();

            if (rooms == null || rooms.Count == 0)
            {
                return NotFound("No rooms found for the specified user.");
            }
            var roomDtos = rooms.Select(RoomDto.FromEntity).ToList();

            return Ok(roomDtos);
        }


        [HttpDelete("{id}/Delete")]
        public async Task<IActionResult> DeleteRoom(Guid id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomExists(Guid id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(Guid id, RoomDto roomDto)
        {
            if (id != roomDto.Id)
            {
                return BadRequest("Room ID mismatch.");
            }

            var existingRoom = await _context.Rooms.FindAsync(id);
            if (existingRoom == null)
            {
                return NotFound("Room not found.");
            }

            // Check if there is another room with the same RoomNo within the same hotel, excluding the current room
            var roomWithSameNumberInHotel = await _context.Rooms
                .AnyAsync(r => r.RoomNo == roomDto.RoomNo
                            && r.HotelId == roomDto.HotelId  // Check within the same hotel
                            && r.Id != id);  // Exclude the current room being updated

            if (roomWithSameNumberInHotel)
            {
                return Conflict("Room with the same room number already exists in this hotel.");
            }

            // Update the properties of the existing room with the values from the DTO
            existingRoom.RoomNo = roomDto.RoomNo;
            existingRoom.Price = roomDto.Price;
            existingRoom.IsAvailable = roomDto.IsAvailable;

            // Update the HotelId if needed (this can be skipped if HotelId should remain unchanged)
            existingRoom.HotelId = roomDto.HotelId;

            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpGet("GetRoomsByHotel/{hotelId}")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRoomsByHotel(Guid hotelId)
        {
            var rooms = await _context.Rooms
                                      .Where(r => r.HotelId == hotelId)
                                      .ToListAsync();

            if (rooms == null || rooms.Count == 0)
            {
                return NotFound();
            }

            var roomDtos = rooms.Select(RoomDto.FromEntity).ToList();
            return Ok(roomDtos);
        }
        [HttpGet("Exist-Bookings/{id}")]
        public async Task<IActionResult> CheckExistBooking(Guid id)
        {
            var room = await _context.Rooms.FindAsync(id)
        ;
            if (room == null)
            {
                return NotFound("Room not found.");
            }
            var hasActiveBookings = await _context.Bookings
                .AnyAsync(b => b.RoomId == id && b.Status == 1);

            if (hasActiveBookings)
            {
                return BadRequest("This room is associated with active bookings and cannot be deleted.");
            }
            return Ok("true");
        }
    }
}
