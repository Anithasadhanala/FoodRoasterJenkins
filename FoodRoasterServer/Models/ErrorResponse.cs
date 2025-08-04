namespace FoodRoasterServer.Models
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Title { get; set; }
        public string ErrorMessage { get; set; }
    }
}
