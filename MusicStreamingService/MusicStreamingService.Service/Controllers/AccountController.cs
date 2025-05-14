using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreamingService.BusinessLogic.Services.Users;
using MusicStreamingService.BusinessLogic.Services.Users.Models;
using MusicStreamingService.Service.Controllers.Requests.Users;

namespace MusicStreamingService.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;

    public AccountController(IMapper mapper, IAccountService accountService)
    {
        _mapper = mapper;
        _accountService = accountService;
    }

    [Authorize]
    [HttpPost]
    [Route("{userName}/change_password")]
    public async Task<IActionResult> ChangePassword(string userName, ChangePasswordRequest request)
    {
        var model = _mapper.Map<ChangePasswordModel>(request);
        
        var result = await _accountService.ChangePasswordAsync(userName, model);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }
        
        return Ok("Password changed successfully");
    }

    [Authorize]
    [HttpPost]
    [Route("{userName}/change_email")]
    public async Task<IActionResult> ChangeEmail(string userName, ChangeEmailRequest request)
    {
        var model = _mapper.Map<ChangeEmailModel>(request);
        
        var result = await _accountService.ChangeEmailAsync(userName, model);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }
        
        return Ok("Email changed successfully");
    }

    [Authorize]
    [HttpPost]
    [Route("{userName}/change_username")]
    public async Task<IActionResult> ChangeUserName(string userName, ChangeUserNameRequest request)
    {
        var model = _mapper.Map<ChangeUserNameModel>(request);
        
        var result = await _accountService.ChangeUserNameAsync(userName, model);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }
        
        return Ok("Username changed successfully");
    }
}