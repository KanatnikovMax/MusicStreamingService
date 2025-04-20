namespace MusicStreamingService.Service.Controllers.Responses.Pagination;

public record PaginatedResponse<TCursor, TItems>(
    TCursor? Cursor, 
    List<TItems> Items);