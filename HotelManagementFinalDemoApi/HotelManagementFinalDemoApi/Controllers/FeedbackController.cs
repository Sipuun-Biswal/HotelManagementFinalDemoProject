using HotelManagementFinalDemoApi.Helpers;
using HotelManagementFinalDemoApi.Models.DataBaseDto;
using HotelManagementFinalDemoApi.Models.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementFinalDemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<FeedbackController> _logger;
        public FeedbackController(ApplicationDbContext context, ILogger<FeedbackController>logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<FeedbackDto>>> GetAllFeedback()
        {
           
                var feedbacks = await _context.Feedbacks

                    .Include(f => f.Booking).
                        ThenInclude(f => f.Room)
                        .ThenInclude(f => f.Hotel)
                    .ToListAsync();

                var feedbackDtos = feedbacks.Select(f => FeedbackDto.FromEntity(f)).ToList();
                return Ok(feedbackDtos);
            }


        [HttpGet("{id}/GetByid")]
        public async Task<ActionResult<FeedbackDto>> GetFeedback(Guid id)
        {
           
                var feedback = await _context.Feedbacks

                    .Include(f => f.Booking)
                    .ThenInclude(f => f.Room)
                    .ThenInclude(f => f.Hotel)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (feedback == null)
                {
                    return NotFound();
                }

                return Ok(FeedbackDto.FromEntity(feedback));
            
          

        }
        [HttpPost("SubmitFeedback")]
        public async Task<ActionResult<FeedbackDto>> PostFeedback(FeedbackDto feedbackDto)
        {
            try
            {
                var feedback = FeedbackDto.ToEntity(feedbackDto);
                feedback.Id = Guid.NewGuid();
                _context.Feedbacks.Add(feedback);
                 _context.SaveChanges();

                return CreatedAtAction("GetFeedback", new { id = feedback.Id }, FeedbackDto.FromEntity(feedback));
            }
            catch (Exception ex) { 
                _logger.LogError(ex,$"Error occured while submit the Feedback from controller{nameof(PostFeedback)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Error");
             }
}

        [HttpPut("{id}/Update")]
        public async Task<IActionResult> PutFeedback(Guid id, FeedbackDto feedbackDto)
        {
            
                if (id != feedbackDto.Id)
                {
                    return BadRequest();
                }

                var feedback = await _context.Feedbacks.FindAsync(id);

                if (feedback == null)
                {
                    return NotFound();
                }

                feedback.FeedbackText = feedbackDto.FeedbackText;
                feedback.Rating = feedbackDto.Rating;
                feedback.BookingId = feedbackDto.BookingId;

                _context.Entry(feedback).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            
            
        }

        [HttpDelete("{id}/Delete")]
        public async Task<IActionResult> DeleteFeedback(Guid id)
        {
              var feedback = await _context.Feedbacks.FindAsync(id);
                if (feedback == null)
                {
                    return NotFound();
                }

                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();

                return NoContent();
            
            
        }
        [HttpGet("GetFeedbackByHotel/{hotelId}")]
        public async Task<ActionResult<IEnumerable<FeedbackDto>>> GetFeedbackByHotel(Guid hotelId)
        {
        
            var hotelFeedback = await _context.Feedbacks
                .Include(f => f.Booking)          
                .ThenInclude(b => b.Room)     
                .ThenInclude(r => r.Hotel)       
                .Where(f => f.Booking.Room.HotelId == hotelId) 
                .ToListAsync();

            var feedbackDtos = hotelFeedback.Select(FeedbackDto.FromEntity).ToList();

            return Ok(feedbackDtos);
        }

    }
}
