using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Users.Models;
using MusicStreamingService.Service.Controllers.Requests.Users;

namespace MusicStreamingService.Service.Mapper;

public class UsersServiceProfile : Profile
{
    public UsersServiceProfile()
    {
        CreateMap<RegisterUserRequest, RegisterUserModel>();
        CreateMap<LoginUserRequest, LoginUserModel>();
        CreateMap<ChangeUserNameRequest, ChangeUserNameModel>();
        CreateMap<ChangePasswordRequest, ChangePasswordModel>();
        CreateMap<ChangeEmailRequest, ChangeEmailModel>();
    }
}