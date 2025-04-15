namespace Captive.Model.Response
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; }

        public ErrorResponse(string message, int statusCode = 400, string errorCode = null)
        {
            Message = message;
            StatusCode = statusCode;
            ErrorCode = errorCode ?? "GENERAL_ERROR";
        }
    }
}
