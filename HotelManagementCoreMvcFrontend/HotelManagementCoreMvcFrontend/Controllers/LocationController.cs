using HotelManagementCoreMvcFrontend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HotelManagementCoreMvcFrontend.Controllers
{
    public class LocationController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7119/api/";
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public LocationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<JsonResult> GetCountry()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}Location/Country");
            if (response.IsSuccessStatusCode)
            {
                var jsonResult = JsonSerializer.Deserialize<List<Country>>(await response.Content.ReadAsStringAsync(), JsonOptions);
                return new JsonResult(jsonResult);
            }
            return new JsonResult(null);
        }
        // Get states by countryId
        public async Task<JsonResult> GetStatesByCountry(int countryId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}Location/States/{countryId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonResult = JsonSerializer.Deserialize<List<State>>(await response.Content.ReadAsStringAsync(), JsonOptions);
                return new JsonResult(jsonResult);
            }
            return new JsonResult(null);
        }
        // Get cities by stateId
        public async Task<JsonResult> GetCitiesByState(int stateId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}Location/cities/{stateId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonResult = JsonSerializer.Deserialize<List<City>>(await response.Content.ReadAsStringAsync(), JsonOptions);
                return new JsonResult(jsonResult);
            }
            return new JsonResult(null);
        }

    }
}
