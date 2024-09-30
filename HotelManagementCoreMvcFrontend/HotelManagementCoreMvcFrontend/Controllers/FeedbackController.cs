using HotelManagementCoreMvcFrontend.Models;
using HotelManagementCoreMvcFrontend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace HotelManagementCoreMvcFrontend.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7119/api/";
        public FeedbackController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [HttpGet]
        public IActionResult SubmitFeedback()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(Guid id, Feedback model)
        {
            if (ModelState.IsValid)
            {
                var feedback = new Feedback
                {
                    Id = Guid.Empty,
                    BookingId = id,
                    FeedbackText = model.FeedbackText,
                    Rating = model.Rating,
                };

                var jsonData = JsonConvert.SerializeObject(feedback);
                var content = new StringContent(jsonData, Encoding.UTF8,"application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}Feedback/SubmitFeedback",content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SubmitFeedback"] = "Feedback Submited Succesfully";
                    return RedirectToAction("Dashboard","Home");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, errorMessage);
                }

            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}Feedback/GetAll");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var feedbacks = JsonConvert.DeserializeObject<List<Feedback>>(jsonData);
                return View(feedbacks);
            }
            return View(new List<Feedback>());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}Feedback/{id}/GetByid");
            if (response.IsSuccessStatusCode)
            {
                var feedback = await response.Content.ReadFromJsonAsync<Feedback>();
                return View(feedback);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                var content = new StringContent(JsonConvert.SerializeObject(feedback), Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_baseUrl}Feedback/{feedback.Id}/Update", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Error updating feedback.");
            }

            return View(feedback);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}Feedback/{id}/Delete");
            if (response.IsSuccessStatusCode)
            {
                var feedback = await response.Content.ReadFromJsonAsync<Feedback>();
                return View(feedback);
            }

            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Feedback feedback)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}Hotel/{feedback.Id}/Delete");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error deleting feedback.");
            return RedirectToAction(nameof(Delete), new { feedback.Id });
        }

        public async Task<IActionResult> FeedbacsByHotel()
        {

            var userIdString = HttpContext.Session.GetString("UserId");

            if (!Guid.TryParse(userIdString, out var userId))
            {
                TempData["Error"] = "You must be logged in to create a room.";
                return RedirectToAction("Login", "Authentication");
            }
            var response = await _httpClient.GetAsync($"{_baseUrl}Hotel/ByUser/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var hotel = await response.Content.ReadFromJsonAsync<Hotel>();
                var hotelId = hotel.Id;
                var response2 = await _httpClient.GetAsync($"{_baseUrl}Feedback/GetFeedbackByHotel/{hotelId}");
                if (response2.IsSuccessStatusCode)
                {
                    var jsonData = await response2.Content.ReadAsStringAsync();
                    var feedbacks = JsonConvert.DeserializeObject<List<Feedback>>(jsonData);
                    return View(feedbacks);
                }
            }
            return NotFound();
        }
    }
}
