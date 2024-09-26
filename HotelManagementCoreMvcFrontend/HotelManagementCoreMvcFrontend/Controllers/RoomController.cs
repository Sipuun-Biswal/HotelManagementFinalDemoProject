using HotelManagementCoreMvcFrontend.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;


namespace HotelManagementCoreMvcFrontend.Controllers
{
    public class RoomController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7119/api/";
        public RoomController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<JsonResult> GetHotel()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}Hotel/All");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var hotels = JsonConvert.DeserializeObject<List<Hotel>>(jsonData);
                return new JsonResult(hotels);
            }
            return new JsonResult(null);
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}Room/AllRoom");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var room = JsonConvert.DeserializeObject<List<Room>>(jsonData);
                return View(room);
            }
            return View(new List<Room>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create( Room room)
         {
            if (ModelState.IsValid)
            {
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(room), Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync($"{_baseUrl}Room/CreateRoom", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Room Success"] = "Room Created Succesfully";
                        return RedirectToAction(nameof(Index));
                    }

                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, errorMessage);


            }

            return View(room);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // Call API to get user by ID for editing
            var response = await _httpClient.GetAsync($"{_baseUrl}Room/{id}");
            if (response.IsSuccessStatusCode)
            {
                var room = await response.Content.ReadFromJsonAsync<Room>();
                return View(room);
            }

            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( Room room)
        {


            if (ModelState.IsValid)
            {
                // Handle Profile Image Upload
                

                var content = new StringContent(JsonConvert.SerializeObject(room), Encoding.UTF8, "application/json");


                var response = await _httpClient.PutAsync($"{_baseUrl}Room/{room.Id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Error updating user.");
            }

            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}Room/{id}/Delete");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error deleting user.");
            return RedirectToAction(nameof(Delete), new {id});
        }
        [HttpGet]
        public async Task<IActionResult> GetRoomsByHotel(Guid hotelId)
        {
            // Fetch rooms for the selected hotel
            var roomsResponse = await _httpClient.GetAsync($"{_baseUrl}Room/GetRoomsByHotel/{hotelId}");
            var roomsData = await roomsResponse.Content.ReadAsStringAsync();

            if (!roomsResponse.IsSuccessStatusCode)
            {
                return NotFound("No rooms found for the selected hotel.");
            }

            var rooms = JsonConvert.DeserializeObject<List<Room>>(roomsData);
            return View(new List<Room>(rooms));
        }
    }

}
