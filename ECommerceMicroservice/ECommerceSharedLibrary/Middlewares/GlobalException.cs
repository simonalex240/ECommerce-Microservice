using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ECommerceSharedLibrary.Logs.LogException;

namespace ECommerceSharedLibrary.Middlewares
{
    public class GlobalException(RequestDelegate next)
    {
        /*
        The middle ware named GlobalException is a middle ware that helps us catch Exceptions that may have slipped up from local middlewares i.e. try and catch blocks.
        We take in RequestDelegate which I understand is a function that helps us call the next function in the middleware chain. (Expound on this )
        */
        public async Task InvokeAsync(HttpContext context)
        {
            /*
            The invoke function is a function that helps us catch those errors if any exists and helps us forward a scary free message that the users can understand.
            What is the HttpContext class and how does it function ?
            */
            string title = "Error";
            string message = "Sorry, Internal server Error. Kindly try again";
            int statusCode = (int)HttpStatusCode.InternalServerError;

            /*
            The default errors will be the ones that we have predefined above i.e. (title message and statusCode).
            Why do we cast the HttpStatusCode.InternalServerError in to an integer and what type of error would we get if this is not done ?
            */

            try
            {
                await next(context);
                /*
                    We will first await the response from the chain.
                    We then run this response form context through a couple of if statements and check through the Response in the context when the Status codes matches one of our status codes. If it does we reassign the variables to match the out come and call the ModifyHeader with the new items for it to display a scary free message to the User.
                */

                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "Too many request made.";
                    statusCode = StatusCodes.Status429TooManyRequests;

                    await ModifyHeader(context, title, message, statusCode);
                }

                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    message = "You are not authorized to access";
                    statusCode = StatusCodes.Status401Unauthorized;
                    await ModifyHeader(context, title, message, statusCode);
                }

                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of Access";
                    message = "You are not allowed to access";
                    statusCode = StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {
                /*
                If the Exceptions status Code do not match any of the above statusCodes it comes to the catch portion of the code where we first,
                using Logger which is a log exception handler we created, log this error to the developer.
                We then check if the exception matches the TaskCanceledException or TimeoutException exception which means some thing might be wrong with the server.
                (
                    Explain why we do not use the status code and what are TaskCanceledException and TimeoutException exceptions.
                )
                */
                Loggers(ex);

                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of time";
                    message = "Request timeout .. tey again";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }
                /*
                We then call the ModifyHeader method as the default operation.
                */
                await ModifyHeader(context, title, message, statusCode);
            }
        }

        private static async Task ModifyHeader(
            HttpContext context,
            string title,
            string message,
            int statusCode
        )
        {
            context.Response.ContentType = "application/json";
            /*
            The ModifyHeader is the message that creates the scary free message.
            What I do not understand here is the JsonSerializer and ProblemDetails ad finally the CancellationToken. Explain them in detail and what they are in the code for.
             */
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(
                    new ProblemDetails()
                    {
                        Detail = message,
                        Status = statusCode,
                        Title = title,
                    }
                ),
                CancellationToken.None
            );
            return;
        }
    }
}
