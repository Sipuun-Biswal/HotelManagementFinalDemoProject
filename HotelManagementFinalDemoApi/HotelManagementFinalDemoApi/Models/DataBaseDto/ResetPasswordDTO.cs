namespace HotelManagementFinalDemoApi.Models.DataBaseDto
{
    public class ResetPasswordDTO
    {
        public Guid UserId { get; set; }
        public string Token {  get; set; }
        public string NewPassword {  get; set; }
    }
}
