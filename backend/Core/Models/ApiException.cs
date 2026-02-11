namespace AeroTrack.Api.Models;

public class ApiException
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string? Details { get; set; } // Stack trace (only for Dev)

    public ApiException(int statusCode, string message, string? details = null)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }
}