using MusicStreamingService.BusinessLogic.Services.Users.Models;

namespace MusicStreamingService.BusinessLogic.Services.Users;

public interface IAuthService
{
    Task<TokenResponce> RegisterUserAsync(RegisterUserModel model);
    Task<TokenResponce> LoginUserAsync(LoginUserModel model);
    Task<TokenResponce> RefreshTokenAsync(string refreshToken);
}
