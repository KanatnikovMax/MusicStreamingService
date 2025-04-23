using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.Service.Init;

public class IdentityProfileService : IProfileService
{
    private readonly UserManager<User> _userManager;

    public IdentityProfileService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        var roles = await _userManager.GetRolesAsync(user);
        
        context.IssuedClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
        return Task.CompletedTask;
    }
}