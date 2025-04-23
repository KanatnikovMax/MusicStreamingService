using MusicStreamingService.BusinessLogic.Exceptions.Common;

namespace MusicStreamingService.BusinessLogic.Exceptions;

public class EntityAlreadyExistsException(string entityName) : BusinessLogicException(
    $"{entityName} is already exists",
    "ENTITY_ALREADY_EXISTS",
    409);