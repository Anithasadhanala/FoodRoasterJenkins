namespace FoodRoasterServer.Constants
{
    public class AppMessages
    {
        public static class Errors
        {
            public const string ResourceNotFound = "Resource not found!";
            public const string ClientSideGenericErrMessage = "The request contains invalid data. Please check your input and try again.";
            public const string UserEmailExists = "A user with this email already exists.";
            public const string JwtNotFound = "Not found Valid Jwt Token!";
            public const string InvalidRoasterQueryParams = "Invalid period. Only 'current-week' or 'previous-week' is allowed.";
            public const string InvalidEmail = "Invalid email.";
            public const string InvalidPassword = "Invalid Password.";
            public const string InvalidDateModifications = "You cannot modify a past date or today's menu after 9:00 AM.";
        }

        public static class Success
        {
            public const string LoginSuccess = "Successfully Logged Out!";
            public const string DeleteFoodRoasterSuccess = "Successfully Deleted ";

        }

        public static class Validation
        {
            public const string UserAlreadyRegistered = "You have already registered for one or more of the selected days.";
            public const string WeeklyRoasterUpdateValidation = "Menu date must be between Monday and Saturday of the current or next week, Exclude the Holidays.";
            public const string UserRolesValidation = "Invalid role specified. Valid roles are: ADMIN, USER, OTHER";
        }
    }
}
