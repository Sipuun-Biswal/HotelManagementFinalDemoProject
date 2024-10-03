namespace HotelManagementCoreMvcFrontend.ViewModels
{
    public class ResertPasswordViewModel
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
