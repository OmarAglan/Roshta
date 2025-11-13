using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Rosheta.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string ErrorTitle { get; set; } = "Error";
        public string ErrorMessage { get; set; } = "An error occurred while processing your request.";
        public int ErrorStatusCode { get; set; } = 500;

        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(int? statusCode = null)
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            // Get error details from middleware if available
            if (HttpContext.Items.TryGetValue("ErrorTitle", out var title))
            {
                ErrorTitle = title?.ToString() ?? ErrorTitle;
            }

            if (HttpContext.Items.TryGetValue("ErrorMessage", out var message))
            {
                ErrorMessage = message?.ToString() ?? ErrorMessage;
            }

            ErrorStatusCode = statusCode ?? ErrorStatusCode;

            // Set friendly messages based on status code
            if (statusCode.HasValue)
            {
                (ErrorTitle, ErrorMessage) = statusCode.Value switch
                {
                    404 => ("Page Not Found", "The page you're looking for doesn't exist."),
                    403 => ("Access Denied", "You don't have permission to access this resource."),
                    401 => ("Unauthorized", "Please log in to access this resource."),
                    400 => ("Bad Request", "The request was invalid. Please check your input."),
                    500 => ("Server Error", "An unexpected error occurred. Our team has been notified."),
                    503 => ("Service Unavailable", "The service is temporarily unavailable. Please try again later."),
                    _ => (ErrorTitle, ErrorMessage)
                };
            }

            _logger.LogWarning("Error page displayed: {StatusCode} - {Title}", ErrorStatusCode, ErrorTitle);
        }
    }
}

