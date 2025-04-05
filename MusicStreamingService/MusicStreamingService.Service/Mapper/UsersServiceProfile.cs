using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Users.Models;
using MusicStreamingService.Service.Controllers.Users.Models;

namespace MusicStreamingService.Service.Mapper;

public class UsersServiceProfile : Profile
{
    public UsersServiceProfile()
    {
        CreateMap<RegisterUserRequest, RegisterUserModel>();
        CreateMap<LoginUserRequest, LoginUserModel>();
        CreateMap<UpdateUserRequest, UpdateUserModel>();
    }
}