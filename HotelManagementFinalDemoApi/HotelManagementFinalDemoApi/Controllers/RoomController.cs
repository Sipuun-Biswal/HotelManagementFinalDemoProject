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
            var room = RoomDto.ToEntity(roomDto);
            room.Id = Guid.NewGuid(); 
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            roomDto.Id = room.Id; 
            return CreatedAtAction("GetRoom", new { id = roomDto.Id }, roomDto);
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
                return BadRequest();
            }

            var existingRoom = await _context.Rooms.FindAsync(id);
            if (existingRoom == null)
            {
                return NotFound();
            }

            // Update the properties of the existing room with the values from the DTO
            existingRoom.RoomNo = roomDto.RoomNo;
            existingRoom.Price = roomDto.Price;
            existingRoom.IsAvailable = roomDto.IsAvailable;

            // If the HotelId needs to be updated (optional based on your requirements)
            existingRoom.HotelId = roomDto.HotelId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

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

    }
}
