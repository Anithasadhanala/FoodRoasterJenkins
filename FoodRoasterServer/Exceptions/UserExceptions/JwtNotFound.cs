namespace FoodRoasterServer.Exceptions.UserExceptions
{
    public class JwtNotFound : Exception
    {
        public JwtNotFound(string message) : base(message)
        {
        }
    }
}
