using HotelManagementCoreMvcFrontend.Models;
using HotelManagementCoreMvcFrontend.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json;
using System.Text.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static HotelManagementCoreMvcFrontend.ViewModels.ChangePaswordViewModel;

namespace HotelManagementCoreMvcFrontend.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7119/api/";

        public AuthenticationController(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }
        // GET: Show registration page
        [HttpGet]
        public IActionResult Register()
        {
            ModelState.Clear();
            return View();
        }

        // POST: Register the user using API
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
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
        Role = Role.User,
        ProfileImage = profileImagePath  
    };

               
    var jsonData = JsonConvert.SerializeObject(user);
    var content = new StringContent(jsonData, Encoding.UTF8,"application/json");

                
    var response = await _httpClient.PostAsync($"{_baseUrl}Auth/register",content);

    if (response.IsSuccessStatusCode)
    {
        TempData["Registration"] = "Registarion Succesful Please Login";
        TempData["Email"] = model.Email;
        return RedirectToAction("VerifyOtp");
    }
    else
    {
        ModelState.AddModelError("", "Email is already exist.");
    }
               
        }
          
        return View(model);
    }

        [HttpGet]
        public IActionResult Login()
        {
            return View();  
        }
        // POST: Log in using API
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
    if (ModelState.IsValid)
    {
    var jsonData = JsonConvert.SerializeObject(new { model.Email, Password = model.Password });
    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

    var response = await _httpClient.PostAsync($"{_baseUrl}Auth/login", content);

    if (response.IsSuccessStatusCode)
    {
    var responseData = await response.Content.ReadAsStringAsync();
    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseData);
    var token = loginResponse?.Token;
    if (!string.IsNullOrEmpty(token))
    {
    HttpContext.Session.SetString("Token", token);
    var tokenHandler = new JwtSecurityTokenHandler();
    var jwtToken = tokenHandler.ReadJwtToken(token); 

                        
    var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;
    var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
    var name = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
    var ProfileImage = jwtToken.Claims.FirstOrDefault(claim => claim.Type=="Images")?.Value;
    if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(roleClaim) || !string.IsNullOrEmpty(ProfileImage))
    {
        HttpContext.Session.SetString("UserId", userId);
        HttpContext.Session.SetString("Name", name);
        HttpContext.Session.SetString("Role", roleClaim);
        HttpContext.Session.SetString("Image", ProfileImage);
    }
    TempData["LoginSuccess"] = "Login successful! Welcome to your account.";
    return RedirectToAction("Dashboard", "Home");                    }
                    
    }
    else
    {
    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    {
    ModelState.AddModelError("", "Invalid credentials. Please try again.");
    }
    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
    ModelState.AddModelError("", "User not registered. Please sign up first.");
    }
    else
    {
    ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
    }
    }
    }
    return View(model);
    }


        [HttpGet]
        public IActionResult VerifyOtp()
        {
            var email = TempData["Email"]?.ToString();
            ViewBag.Email = email;
            return View();
        }
// POST: Verify OTP using API
[HttpPost]
public async Task<IActionResult> VerifyOtp(OtpViewModel model)
{
    if (ModelState.IsValid)
    {
                var role = HttpContext.Session.GetString("Role");
                var jsonData = JsonConvert.SerializeObject(new { model.Email, Code = model.Code });
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}Auth/OtpVerification",content);

        if (response.IsSuccessStatusCode)
        {
            if (role == "Admin")
            {
                        var Subject = "Resert Your Password";
                        SetAuthorizationHeader(_httpClient);
                        var content1 = new StringContent("", Encoding.UTF8, "application/json");
                        var response2 = await _httpClient.PostAsync($"{_baseUrl}auth/send-email/{model.Email}?subject={Subject}&message=", content1);

                        return RedirectToAction("Index", "User");
                   }
             else  
            {
                return RedirectToAction("Login");
            }
        }

        ModelState.AddModelError("", "OTP verification failed. Invalid OTP or email expired.");
    }

    return View(model);
}

// POST: Log out
public async Task<IActionResult> Logout()
{
    var token = HttpContext.Session.GetString("Token");

    if (!string.IsNullOrEmpty(token))
    {
        var content = new StringContent("", Encoding.UTF8, "application/json");
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        await _httpClient.PostAsync($"{_baseUrl}Auth/logout", content);
    }

    HttpContext.Session.Clear();
    return RedirectToAction("Login");
}
[HttpGet]
public IActionResult ChangePassword()
{
    return View();
}
[HttpPost]
public async Task<IActionResult> ChangePassword(ChangePaswordViewModel model)
{
    if (ModelState.IsValid)
    {
        // Assuming the UserId is stored in session or retrieved elsewhere
        var userIdString = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userIdString))
        {
            return RedirectToAction("Login", "Authentication");
        }

        model.UserId = Guid.Parse(userIdString);

        var changePasswordDto = new ChangePaswordViewModel
        {
            UserId = model.UserId,
            CurrentPassword = model.CurrentPassword,
            NewPassword = model.NewPassword,
            ConfirmNewPassword = model.ConfirmNewPassword
        };

        var jsonContent = JsonConvert.SerializeObject(changePasswordDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}Auth/ChangePassword", content);

        if (response.IsSuccessStatusCode)
        {
            TempData["PasswordMessage"] = "Password changed successfully.";
            return RedirectToAction("Dashboard", "Home");
        }
        var errorMessage = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError(string.Empty, errorMessage);
    }
            
    return View(model);
}
[AcceptVerbs("Get", "Post")]
public async Task<IActionResult> IsEmailAvailable(string email)
{
    if (string.IsNullOrWhiteSpace(email))
    {
        return Json("Email is required.");
    }
    var url = $"{_baseUrl}Auth/EmailCheck?email={email}";
    var response = await _httpClient.GetAsync(url);
    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();
        bool emailExists = bool.TryParse(content, out bool exists) && exists;
        if (emailExists)
        {
            return Json($" This email is already associated with an account.");
        }
    }
    else
    {
        return Json("Error checking email availability.");
    }

    return Json(true);
}
        [HttpGet("reset-password")]
        public IActionResult ResetPassword(Guid userId,string token)
        {
            var passwordResert = new ResertPasswordViewModel
            {
                UserId = userId,
                Token = token
            };
            return View(passwordResert);
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult>ResetPassword(ResertPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var jsonContent = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}auth/reset-password", content);
                if (response.IsSuccessStatusCode) {
                    return RedirectToAction("Login", "Authentication");
                }
            }
            return View(model);
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



    

