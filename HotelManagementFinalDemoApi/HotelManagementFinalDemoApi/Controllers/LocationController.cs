using HotelManagementFinalDemoApi.Models.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementFinalDemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public LocationController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("Country")]
        public async Task<IActionResult> GetAllCountries()
        {
            var countries = await _context.Countries.ToListAsync();
            return Ok(countries);
        }
        [HttpGet("States/{countryId}")]
        public async Task<IActionResult> GetSates(int countryId)
        {
            var states = await _context.States.Where(s => s.CountryId == countryId).ToListAsync();
            return Ok(states); ;
        }
        [HttpGet("cities/{stateId}")]
        public async Task<IActionResult> GetCities( int stateId)
        {
            var cities =await _context.Cities.Where(c => c.StateId == stateId).ToListAsync();

            return Ok(cities);
            
        }

       
    }
}
