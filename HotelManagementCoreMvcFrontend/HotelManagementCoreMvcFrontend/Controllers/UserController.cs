using Microsoft.AspNetCore.Mvc;
using HotelManagementCoreMvcFrontend.ViewModels;
using HotelManagementCoreMvcFrontend.Models;
using System.Text;
using System.Text.Json;

using System.Reflection;
using System.Security.Claims;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Net.Http.Headers;

namespace HotelManagementCoreMvcFrontend.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7119/api/";

        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IActionResult> Index()
        {
            SetAuthorizationHeader(_httpClient);
            var response = await _httpClient.GetAsync($"{_baseUrl}User/GetAll");
            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<List<User>>();
                return View(users);
            }
            return View(new List<User>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
               string? profileImagePath = null;
                if (model.ProfileImage != null && model.ProfileImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfileImage.CopyToAsync(fileStream);
                    }
                    profileImagePath = "/images/" + uniqueFileName;
                }
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,
                    Role = model.Role,
                    ProfileImage = profileImagePath
                };
                var jsonData = JsonSerializer.Serialize(user);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                SetAuthorizationHeader(_httpClient);
                var response = await _httpClient.PostAsync($"{_baseUrl}Auth/register", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Email"] = model.Email;
                    return RedirectToAction("VerifyOtp", "Authentication");

                }

                ModelState.AddModelError("", "Error creating user. Please try again.");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            SetAuthorizationHeader(_httpClient);
            // Call API to get user by ID for editing
            var response = await _httpClient.GetAsync($"{_baseUrl}User/{id}");
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<User>();
                return View(user);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile? image, User user)
        {
            if (ModelState.IsValid)
            {
                SetAuthorizationHeader(_httpClient);
                var existingUserResponse = await _httpClient.GetAsync($"{_baseUrl}User/{user.Id}");
                var existingUser = await existingUserResponse.Content.ReadFromJsonAsync<User>();
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

                    user.ProfileImage = "/images/" + uniqueFileName;
                }
                else
                {
                    user.ProfileImage = existingUser?.ProfileImage;
                }

                // Set UpdatedBy to the current user's ID (replace with your method of fetching the logged-in user)
                // Assuming Identity is used

                var jsonData = JsonSerializer.Serialize(user);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                SetAuthorizationHeader(_httpClient);
                var response = await _httpClient.PutAsync($"{_baseUrl}User/{user.Id}", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccesUpdate"] = "Update Succesfull";
                    return RedirectToAction("Dashboard", "Home");
                    
                }

                ModelState.AddModelError("", "Error updating user.");
            }

            return View(user);
        }

        public async Task<IActionResult> Delete(Guid Id)
        {
            SetAuthorizationHeader(_httpClient);
            var response = await _httpClient.DeleteAsync($"{_baseUrl}User/{Id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Error deleting user.");
            return RedirectToAction(nameof(Delete), new {Id=Id});
        }
        //Check if user associated with bookings or not
        public async Task<IActionResult> DeleteConformation(Guid Id)
        {
           
            var response = await _httpClient.GetAsync($"{_baseUrl}User/Exist-bookings/{Id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Delete", new {id=Id});
            }
            var message = await response.Content.ReadAsStringAsync();
            TempData["Warning"] = message;
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> GetAllManagers()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}User/GetAllManagers");
             var users = await response.Content.ReadFromJsonAsync<List<User>>();
            return Json(users);
        }
        //Set Authorization Header
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