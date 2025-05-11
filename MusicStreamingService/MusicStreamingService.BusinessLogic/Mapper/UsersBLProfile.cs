using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Users.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Mapper;

public class UsersBLProfile : Profile
{
    public UsersBLProfile()
    {
        CreateMap<User, UserModel>();
        CreateMap<RegisterUserModel, User>();
        CreateMap<UpdateUserModel, User>();
        CreateMap<UserAlbum, UserAlbumModel>();
        CreateMap<UserSong, UserSongModel>();
    }
}