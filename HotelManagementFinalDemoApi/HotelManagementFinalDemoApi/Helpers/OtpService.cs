using HotelManagementFinalDemoApi.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementFinalDemoApi.Helpers
{
    public class OtpService
    {
        private readonly ApplicationDbContext _context;

        public OtpService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GenerateOtpAsync(Guid userId)
        {
            var otp = new Random().Next(100000, 999999); // Generate a 6-digit OTP
            var otpEntry = new Otp
            {
                UserId = userId,
                Code = otp,
                ExpTime = DateTime.UtcNow.AddMinutes(10) // OTP valid for 10 minutes
            };

            _context.Otps.Add(otpEntry);
            await _context.SaveChangesAsync();
            return otp;
        }

        public async Task<bool> ValidateOtpAsync(Guid userId, int code)
        {
            var otp = await _context.Otps
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Code == code && o.ExpTime > DateTime.UtcNow);

            if (otp != null)
            {
                _context.Otps.Remove(otp); // OTP used, so remove it
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
