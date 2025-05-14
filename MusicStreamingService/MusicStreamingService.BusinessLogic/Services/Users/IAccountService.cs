using Microsoft.AspNetCore.Identity;
using MusicStreamingService.BusinessLogic.Services.Users.Models;

namespace MusicStreamingService.BusinessLogic.Services.Users;

public interface IAccountService
{
    Task<IdentityResult> ChangePasswordAsync(string userName, ChangePasswordModel model);
    Task<IdentityResult> ChangeEmailAsync(string userName, ChangeEmailModel model);
    Task<IdentityResult> ChangeUserNameAsync(string userName, ChangeUserNameModel model);
}