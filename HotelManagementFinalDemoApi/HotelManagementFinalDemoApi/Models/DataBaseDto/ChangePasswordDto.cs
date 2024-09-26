namespace HotelManagementFinalDemoApi.Models.DataBaseDto
{
    public class ChangePasswordDto
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
