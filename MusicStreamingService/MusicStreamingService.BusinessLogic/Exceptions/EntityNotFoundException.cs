using MusicStreamingService.BusinessLogic.Exceptions.Common;

namespace MusicStreamingService.BusinessLogic.Exceptions;

public class EntityNotFoundException(string entityName, Guid id) : BusinessLogicException(
    $"{entityName} was not found. Id: {id}",
    "ENTITY_NOT_FOUND",
    404);