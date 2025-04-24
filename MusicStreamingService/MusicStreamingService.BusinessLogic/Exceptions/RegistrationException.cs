using MusicStreamingService.BusinessLogic.Exceptions.Common;

namespace MusicStreamingService.BusinessLogic.Exceptions;

public class RegistrationException : BusinessLogicException
{
    public RegistrationException(string message) : base(message, "REGISTRATION_EXCEPTION", 400)
    {
    }
}