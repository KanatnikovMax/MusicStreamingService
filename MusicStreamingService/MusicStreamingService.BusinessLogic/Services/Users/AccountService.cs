using Microsoft.AspNetCore.Identity;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Users.Models;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.BusinessLogic.Services.Users;

public class AccountService : IAccountService
{
    private readonly UserManager<User> _userManager;

    public AccountService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> ChangePasswordAsync(string userName, ChangePasswordModel model)
    {
        var user = await _userManager.FindByNameAsync(userName)
                   ?? throw new EntityNotFoundException("User");
        
        return await _userManager.ChangePasswordAsync(
            user,
            model.CurrentPassword,
            model.NewPassword);
    }

    public async Task<IdentityResult> ChangeEmailAsync(string userName, ChangeEmailModel model)
    {
        var user = await _userManager.FindByNameAsync(userName)
                   ?? throw new EntityNotFoundException("User");

        if (!await _userManager.CheckPasswordAsync(user, model.Password))
        {
            throw new AccessDeniedException("Incorrect password");
        }

        var email = model.Email;
        if (!string.IsNullOrEmpty(email))
        {
            user.Email = email;
            user.NormalizedEmail = _userManager.NormalizeEmail(email);
        }
        
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> ChangeUserNameAsync(string userName, ChangeUserNameModel model)
    {
        var user = await _userManager.FindByNameAsync(userName)
                   ?? throw new EntityNotFoundException("User");

        if (!await _userManager.CheckPasswordAsync(user, model.Password))
        {
            throw new AccessDeniedException("Incorrect password");
        }

        var newUserName = model.UserName;
        if (!string.IsNullOrEmpty(userName))
        {
            var existingUser = await _userManager.FindByNameAsync(newUserName);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                throw new EntityAlreadyExistsException("User");
            }
            user.UserName = newUserName;
        }
        
        return await _userManager.UpdateAsync(user);
    }
}