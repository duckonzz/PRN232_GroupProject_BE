using GenderHealthCare.Core.Models;

namespace GenderHealthCare.Core.Exceptions
{
    public class ErrorException : Exception
    {
        public int StatusCode { get; }

        public ErrorDetail ErrorDetail { get; }

        public ErrorException(int statusCode, string message)
        {
            StatusCode = statusCode;
            ErrorDetail = new ErrorDetail
            {
                ErrorMessage = message
            };
        }

        public ErrorException(int statusCode, string errorCode, string message)
        {
            StatusCode = statusCode;
            ErrorDetail = new ErrorDetail
            {
                ErrorCode = errorCode,
                ErrorMessage = message
            };
        }

        public ErrorException(int statusCode, ErrorDetail errorDetail)
        {
            StatusCode = statusCode;
            ErrorDetail = errorDetail;
        }
    }
}
