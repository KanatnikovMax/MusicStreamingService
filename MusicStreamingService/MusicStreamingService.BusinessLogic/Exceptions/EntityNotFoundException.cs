using MusicStreamingService.BusinessLogic.Exceptions.Common;

namespace MusicStreamingService.BusinessLogic.Exceptions;

public class EntityNotFoundException : BusinessLogicException
{
    public EntityNotFoundException(string entityName, Guid id) : base($"{entityName} was not found. Id: {id}",
        "ENTITY_NOT_FOUND",
        404)
    {}

    public EntityNotFoundException(string entityName) : base($"{entityName} was not found.",
        "ENTITY_NOT_FOUND",
        404)
    {}
}