using AutoMapper;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.Service.Controllers.Requests.Pagination;
using MusicStreamingService.Service.Controllers.Responses.Pagination;

namespace MusicStreamingService.Service.Mapper;

public class PaginationServiceProfile : Profile
{
    public PaginationServiceProfile()
    {
        CreateMap(typeof(PaginationRequest<>), typeof(PaginationParams<>));
        CreateMap(typeof(CursorResponse<,>), typeof(PaginatedResponse<,>));
    }
}