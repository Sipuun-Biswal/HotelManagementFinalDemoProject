using HotelManagementFinalDemoApi.Helpers;
using HotelManagementFinalDemoApi.Models.DataBaseDto;
using HotelManagementFinalDemoApi.Models.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace HotelManagementFinalDemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly TokenHelper _tokenHelper;
        private readonly ILogger<AuthController> _logger;
        private readonly EmailService _emailService;
        private readonly OtpService _otpService;
        public AuthController(ApplicationDbContext context, TokenHelper tokenHelper, ILogger<AuthController> logger, EmailService emailService, OtpService otpService)
        {
            _context = context;
            _tokenHelper = tokenHelper;
            _logger = logger;
            _emailService = emailService;
            _otpService = otpService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (existingUser != null)
            {

                return Conflict("User with this email already exists.");
            }

            var user = UserDto.ToEntity(userDto);
            user.PasswordHash = PasswordHash.HashPassword(userDto.Password);
            user.Id = Guid.NewGuid();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var otp = await _otpService.GenerateOtpAsync(user.Id);
            var body = $"Your OTP code is: {otp}";

            await _emailService.SendEmailAsync(userDto.Email, "Email Verification", body);

            return Ok(new { Message = "Registration successful. Please check your email for OTP." });
        }




        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            string Password = PasswordHash.HashPassword(loginDto.Password);
            if (user == null || user.PasswordHash != Password || user.IsEmailVerified==false || user.IsActive==false)
            {
                return Unauthorized("Invalid credentials Or User have not active");
            }

            var token = _tokenHelper.GenerateToken(user);
            user.Token = token;
            _context.Users.Update(user);
             _context.SaveChanges();

            return Ok(new { Token = token });
        }


        [HttpPost("logout")]
        [Authorize(Roles="User")]
        public async Task<IActionResult> Logout()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users.FindAsync(Guid.Parse(userId));

            if (user == null)
            {
                return BadRequest("User not found.");
            }


            user.Token = null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();


            return Ok("Logged out successfully.");
        }

        [HttpPost("OtpVerification")]
        public async Task<IActionResult> Verifyotp([FromBody] OtpVerificationDto otpVerificationDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == otpVerificationDto.Email);
            if (user == null)
            {
                return BadRequest(new { Message = "User have not exist" });
            }
            var isValid = await _otpService.ValidateOtpAsync(user.Id, otpVerificationDto.Code);
            if (isValid) {
                user.IsEmailVerified = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "User created succesfuly" });
            }
            return BadRequest(new { Message = "Invalid OTP or Email Exparied" });

        }
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users.FindAsync(changePasswordDto.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            string password = PasswordHash.HashPassword(changePasswordDto.CurrentPassword);
            // Verify the current password
            if ((user.PasswordHash!=password))
            {
                return BadRequest("Current password is incorrect.");
            }
            // Hash the new password
            user.PasswordHash = PasswordHash.HashPassword(changePasswordDto.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Password changed successfully.");
        }
        //Remote Validation Check For Email.
        [HttpGet("EmailCheck")]
        public async Task<bool> IsEmailAvailable(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
              return false;
            }
            var emailExists = await _context.Users.AnyAsync(u => u.Email == email);
            return emailExists;
        }
        //Send Email For Resert Password
        [HttpPost("send-email/{email}")]
        public async Task<IActionResult> Sendmail(string email, string subject, string? message)
        {
          
                var user = await _context.Users.FirstOrDefaultAsync(u=>u.Email==email);
                if (User.IsInRole("Admin"))
                {
                    // Generate a secure token
                    var token = PasswordHash.GenerateSecureToken();
                    var baseUrl = "https://localhost:5167/reset-password"; // URL to your MVC controller

                    // Construct the reset password link
                    var resetLink = $"{baseUrl}?userId={user.Id}&token={HttpUtility.UrlEncode(token)}";

                    // Prepare the email body
                    var text = $@"
                           Mr/Mrs: {user.FirstName} {user.LastName},

                           Welcome to  Hotel Room Booking! You have been added  by the admin.
                           Email:{user.Email}
                           To get started, please set your password below:
                           {resetLink}";

                    // Create a new password reset token
                    var passwordResetToken = new ResertPassword()
                    {
                       
                        UserId = user.Id,
                        Token = token,
                        ExiparyTime = DateTime.Now.AddMinutes(5) // Set the expiration time to 5 minutes
                    };
                    // Store the token in your database
                     _context.ResertPasswords.Add(passwordResetToken);
                     await _context.SaveChangesAsync();
                // Send the email with the constructed message
                await _emailService.SendEmailAsync(email, subject, text);
                }
                else
                {

                    await _emailService.SendEmailAsync(email, subject, message);
                }
                return Ok("Message has been sent to your email");

        }
        //Resert Password 
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _context.ResertPasswords.FirstOrDefaultAsync(U => U.UserId == model.UserId && U.Token == model.Token);

            if (token == null)
            {
                return BadRequest("Token not found.");
            }

            if (token.ExiparyTime <= DateTime.Now)
            {
                return BadRequest("Token has expired.");
            }

            // Remove the token once it's valid
            _context.ResertPasswords.Remove(token);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Hash the new password
            try
            {
                user.PasswordHash = PasswordHash.HashPassword(model.NewPassword);
            }
            catch (Exception ex)
            {
                return BadRequest($"Password hashing failed: {ex.Message}");
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Password has been reset successfully.");
        }

    }
}
