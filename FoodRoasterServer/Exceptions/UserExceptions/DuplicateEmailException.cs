namespace FoodRoasterServer.Exceptions.UserExceptions
{

    public class DuplicateEmailException : Exception
    {
        public DuplicateEmailException(string message) : base(message)
        {
        }
    }
}
