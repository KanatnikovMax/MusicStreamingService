namespace MusicStreamingService.BusinessLogic.Exceptions.Common;

public class BusinessLogicException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }
    
    public BusinessLogicException(
        string message, 
        string errorCode,
        int statusCode = 400 
    ) : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}