using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Users.Models;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.BusinessLogic.Mapper;

public class UsersBLProfile : Profile
{
    public UsersBLProfile()
    {
        CreateMap<User, UserModel>();
        CreateMap<RegisterUserModel, User>();
        CreateMap<UpdateUserModel, User>();
    }
}