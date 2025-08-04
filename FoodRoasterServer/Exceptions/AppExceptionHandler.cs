using FoodRoasterServer.Constants;
using FoodRoasterServer.Exceptions.UserExceptions;
using FoodRoasterServer.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace FoodRoasterServer.Exceptions
{
    public class AppExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception, CancellationToken cancellationToken)
        {
            var response = new ErrorResponse();

            if (exception is ValidationException || exception is ArgumentException || exception is InvalidOperationException ||
                exception is DivideByZeroException || exception is KeyNotFoundException || exception is DuplicateEmailException ||
                exception is JwtNotFound || exception is UseRegisteredForFoodException)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Title = "Bad Request";
                response.ErrorMessage =
                    !string.IsNullOrWhiteSpace(exception.Message)
                                    ? exception.Message
                                    : AppMessages.Errors.ClientSideGenericErrMessage;
            }
            else
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.ErrorMessage = exception.Message;
                response.Title = "Something went wrong!";
            }
            httpContext.Response.StatusCode = response.StatusCode;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }
    }
}
