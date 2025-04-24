using MusicStreamingService.BusinessLogic.Exceptions.Common;

namespace MusicStreamingService.BusinessLogic.Exceptions;

public class AccessDeniedException : BusinessLogicException
{
    public AccessDeniedException(string message) : base(message, "ACCESS_DENIED_EXCEPTION", 400)
    {
    }
}