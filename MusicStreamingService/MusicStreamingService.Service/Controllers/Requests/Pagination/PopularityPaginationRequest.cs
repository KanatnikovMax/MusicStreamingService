using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.Service.Controllers.Requests.Pagination;

public record PopularityPaginationRequest(
    long? CursorPlayCount,
    DateTime? CursorCreatedAt,
    int PageSize)
{
    public PaginationParams<PopularityCursor> ToPaginationParams()
    {
        return new PaginationParams<PopularityCursor>
        {
            Cursor = CursorPlayCount is not null && CursorCreatedAt is not null
                ? new PopularityCursor(CursorPlayCount.Value, CursorCreatedAt.Value)
                : null,
            PageSize = PageSize
        };
    }
}
