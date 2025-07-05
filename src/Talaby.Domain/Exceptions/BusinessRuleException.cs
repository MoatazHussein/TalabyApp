namespace Talaby.Domain.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }

        public BusinessRuleException(string message, int statusCode = 422, string errorCode = null)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public BusinessRuleException(string message, Exception innerException, int statusCode = 422, string errorCode = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}
