namespace HotelManagementFinalDemoApi.Models.DataModels
{
    public class ErrorLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Message { get; set; }
        public string Exception { get; set; }
        public string? Source { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
