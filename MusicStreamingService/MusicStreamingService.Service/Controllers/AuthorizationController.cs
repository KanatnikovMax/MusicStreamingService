using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MusicStreamingService.BusinessLogic.Services.Users;
using MusicStreamingService.BusinessLogic.Services.Users.Models;
using MusicStreamingService.Service.Controllers.Requests.Users;

namespace MusicStreamingService.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IAuthService _authService;

    public AuthorizationController(IMapper mapper, IAuthService authService)
    {
        _mapper = mapper;
        _authService = authService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<TokenResponce>> RegisterUser([FromForm] RegisterUserRequest request)
    {
        var registerModel = _mapper.Map<RegisterUserModel>(request);
        var tokens = await _authService.RegisterUserAsync(registerModel);
        return Ok(tokens);
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<TokenResponce>> LoginUser([FromForm] LoginUserRequest request)
    {
        var authorizeModel = _mapper.Map<LoginUserModel>(request);
        var tokens = await _authService.LoginUserAsync(authorizeModel);
        return Ok(tokens);
    }
}