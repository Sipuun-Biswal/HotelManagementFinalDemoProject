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

    }
}
