namespace MusicStreamingService.Service.Controllers.Requests.Pagination;

public record PaginationRequest<T>(
    T? Cursor,
    int PageSize);