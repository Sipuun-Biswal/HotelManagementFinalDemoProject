using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HotelManagementCoreMvcFrontend.Models;
using System.Text;
using System.Security.Claims;

namespace HotelManagementCoreMvcFrontend.Controllers
{
    public class HotelController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7119/api/";
        public HotelController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        // GET: Hotels/Index - Fetch all hotels
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}Hotel/All");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var hotels = JsonConvert.DeserializeObject<List<Hotel>>(jsonData);
                return View(hotels);
            }
            return View(new List<Hotel>());
        }
     //Create hotel
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile? image ,Hotel hotel)

        {

            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)

                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");


                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    hotel.HotelImage = "/images/" + uniqueFileName;
                }

                var userIdString = HttpContext.Session.GetString("UserId");

                if (Guid.TryParse(userIdString, out var userId))
                {
                    // Assign the logged-in user's ID to the HotelDto
                    hotel.UserId = userId;

                    var jsonContent = new StringContent(JsonConvert.SerializeObject(hotel), Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync($"{_baseUrl}Hotel", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["HotelSuccess"] = "Hotel Created Succesfully";
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError("", "Error occurred while creating the hotel.");
                }
                else
                {
                    ModelState.AddModelError("", "Hotel have not created .Try Again.");
                }
            }

            return View(hotel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // Call API to get user by ID for editing
            var response = await _httpClient.GetAsync($"{_baseUrl}Hotel/{id}");
            if (response.IsSuccessStatusCode)
            {
                var hotel = await response.Content.ReadFromJsonAsync<Hotel>();
                return View(hotel);
            }

            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(IFormFile? image, Hotel hotel)
        {


            if (ModelState.IsValid)
            {
                // Handle Profile Image Upload
                if (image != null && image.Length > 0)

                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");


                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    hotel.HotelImage = "/images/" + uniqueFileName;
                }

                // Set UpdatedBy to the current user's ID (replace with your method of fetching the logged-in user)
                // Assuming Identity is used

                var content = new StringContent(JsonConvert.SerializeObject(hotel), Encoding.UTF8, "application/json");
              

                var response = await _httpClient.PutAsync($"{_baseUrl}Hotel/{hotel.Id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Something Else User Have Not Updated.");
            }

            return View(hotel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}Hotel/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error deleting user.");
            return RedirectToAction(nameof(Delete), new {id});
        }

        [HttpGet]
        public async Task<IActionResult> ShowAllHotel()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}Hotel/All");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var hotels = JsonConvert.DeserializeObject<List<Hotel>>(jsonData);
                return View(hotels);
            }
            return View(new List<Hotel>());
        }
    }
}
