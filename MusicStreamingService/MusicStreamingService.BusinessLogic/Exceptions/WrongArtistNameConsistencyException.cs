using MusicStreamingService.BusinessLogic.Exceptions.Common;

namespace MusicStreamingService.BusinessLogic.Exceptions;

public class WrongArtistNameConsistencyException : BusinessLogicException
{
    public WrongArtistNameConsistencyException() : base("Wrong Artist Name Consistency",
        "WRONG_ARTIST_NAME_CONSISTENCY",
        409)
    {}
}