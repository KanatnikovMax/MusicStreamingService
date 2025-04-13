using AutoMapper;
using MusicStreamingService.DataAccess.Repositories.Interfaces;

namespace MusicStreamingService.BusinessLogic.Services.Users;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IMapper _mapper;

    public UsersService(IUsersRepository usersRepository, IMapper mapper)
    {
        _usersRepository = usersRepository;
        _mapper = mapper;
    }
}