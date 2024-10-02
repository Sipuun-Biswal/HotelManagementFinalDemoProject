using HotelManagementCoreMvcFrontend.Models;
using HotelManagementCoreMvcFrontend.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HotelManagementCoreMvcFrontend.Controllers
{
    public class BookingController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7119/api/";
        public BookingController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        [HttpGet]
        public IActionResult CreateBooking()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateBooking(Guid id, BookingViewModel model)
        {
           
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "Authentication");
            }

            if (ModelState.IsValid)
            {
                Guid.TryParse(userIdString, out var userId);
                var booking = new Booking
                {
                    RoomId = id,
                    UserId = userId,
                    CheckInDate = (DateTime)model.CheckInDate,
                    CheckOutDate =(DateTime) model.CheckOutDate,
                    Status=Status.Pending,
                    CreatedBy=userId,


                };
                SetAuthorizationHeader(_httpClient);
                var jsonData = JsonConvert.SerializeObject(booking);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}Booking/createBooking", content);

                if (response.IsSuccessStatusCode)
                {

                    TempData["BookingSuccesful"] = "Booking Succesfuly Created";
                    return RedirectToAction(nameof(GetBookingsByUser), new { id = userId });
                   
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, errorMessage);
                }

            }
            
            return View(model);
        }
        //Get All booking
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            SetAuthorizationHeader(_httpClient);
            var response = await _httpClient.GetAsync($"{_baseUrl}Booking/GetAllBooking");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var bookings = JsonConvert.DeserializeObject<List<Booking>>(jsonData);
                return View(bookings);
            }
            return View(new List<Booking>());
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            SetAuthorizationHeader(_httpClient);
            var response = await _httpClient.GetAsync($"{_baseUrl}Booking/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var booking = JsonConvert.DeserializeObject<List<Booking>>(jsonData);
                return View(booking);
            }
            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            SetAuthorizationHeader(_httpClient);
            var response = await _httpClient.GetAsync($"{_baseUrl}Booking/GetBookingById/{id}");
            if (response.IsSuccessStatusCode)
            {
                var booking = await response.Content.ReadFromJsonAsync<Booking>();
                return View(booking);
            }

            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Booking booking)
        {
            if (ModelState.IsValid)
            {
                var role = HttpContext.Session.GetString("Role");
                var content = new StringContent(JsonConvert.SerializeObject(booking), Encoding.UTF8, "application/json");
                SetAuthorizationHeader(_httpClient);
                var response = await _httpClient.PutAsync($"{_baseUrl}Booking/{booking.Id}", content);
                if (response.IsSuccessStatusCode)
                {
                    if (role == "Admin")
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else if (role == "Manager")
                    {
                        return RedirectToAction(nameof(BookingsByHotel));
                    }
                }

                ModelState.AddModelError("", "Error updating user.");
            }

            return View(booking);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
                var role = HttpContext.Session.GetString("Role");
                var userId = HttpContext.Session.GetString("UserId");
            SetAuthorizationHeader(_httpClient);
            var response = await _httpClient.DeleteAsync($"{_baseUrl}Booking/{id}");
            if (response.IsSuccessStatusCode)
            {
                if (role == "Admin")
                {
                    return RedirectToAction(nameof(Index));
                }
                else if (role == "Manager")
                {
                    return RedirectToAction(nameof(BookingsByHotel));
                }
                else if(role == "User")
                {
                    return RedirectToAction(nameof(GetBookingsByUser), new {userId});
                }
            }
            ModelState.AddModelError("", "Error deleting user.");
            return RedirectToAction(nameof(Delete), new {id});
        }
        [HttpGet]
        public async Task<IActionResult> GetBookingsByUser(Guid id)
        {
            SetAuthorizationHeader(_httpClient);
            var response = await _httpClient.GetAsync($"{_baseUrl}Booking/GetBookingsByUser/{id}");

            var bookingdata = await response.Content.ReadAsStringAsync();
            var bookings = JsonConvert.DeserializeObject<List<Booking>>(bookingdata);
            if(bookings==null || bookings.Count == 0)
            {
                ViewBag.Message = "No Bookings yet";
                return View(new List<Booking>());
            }
            return View(new List<Booking>(bookings));  
        }

        public async Task<IActionResult> BookingsByHotel()
        {
          

            var userIdString = HttpContext.Session.GetString("UserId");

            if (!Guid.TryParse(userIdString, out var userId))
            {
                TempData["Error"] = "You must be logged in to create a room.";
                return RedirectToAction("Login", "Authentication");
            }
            SetAuthorizationHeader(_httpClient);
            var response = await _httpClient.GetAsync($"{_baseUrl}Hotel/ByUser/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var hotel = await response.Content.ReadFromJsonAsync<Hotel>();
                var hotelId = hotel.Id;
                var response2 = await _httpClient.GetAsync($"{_baseUrl}Booking/GetBookingsByHotel/{hotelId}");
                if (response2.IsSuccessStatusCode)
                {
                    var jsonData = await response2.Content.ReadAsStringAsync();
                    var bookings = JsonConvert.DeserializeObject<List<Booking>>(jsonData);
                    return View(bookings);
                }
            }
            return NotFound();
        }
        private void SetAuthorizationHeader(HttpClient httpClient)
        {
            var token = HttpContext.Session.GetString("Token");
           if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
