using System.Security.Authentication;
using AutoMapper;
using Duende.IdentityModel.Client;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Users.Models;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.BusinessLogic.Services.Users;

public class AuthService : IAuthService
{
    private readonly MusicServiceDbContext _context;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMapper _mapper;
    private readonly string _identityServerUri;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public AuthService(MusicServiceDbContext context, SignInManager<User> signInManager, UserManager<User> userManager, 
        IHttpClientFactory httpClientFactory, IMapper mapper, 
        string identityServerUri, string clientId, string clientSecret)
    {
        _context = context;
        _signInManager = signInManager;
        _userManager = userManager;
        _httpClientFactory = httpClientFactory;
        _mapper = mapper;
        _identityServerUri = identityServerUri;
        _clientId = clientId;
        _clientSecret = clientSecret;
    }

    public async Task<TokenResponce> RegisterUserAsync(RegisterUserModel model)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user is not null)
            {
                throw new EntityAlreadyExistsException("User");
            }

            user = _mapper.Map<User>(model);

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                throw new RegistrationException(
                    string.Join(Environment.NewLine, createResult.Errors.Select(e => e.Description)) );
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "user");
            if (!roleResult.Succeeded)
            {
                throw new RegistrationException(
                    string.Join(Environment.NewLine, roleResult.Errors.Select(e => e.Description)));
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            var client = _httpClientFactory.CreateClient();
            var discoveryDocument = await client.GetDiscoveryDocumentAsync(_identityServerUri);
            if (discoveryDocument.IsError)
            {
                throw new AuthenticationException("Identity server error");
            }

            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                GrantType = GrantType.ResourceOwnerPassword,
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                UserName = user.UserName,
                Password = model.Password,
                Scope = "api offline_access"
            });
            
            if (tokenResponse.IsError)
            {
                throw new AuthenticationException("Identity server error");
            }
        
            return new TokenResponce
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<TokenResponce> LoginUserAsync(LoginUserModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user is null)
        {
            throw new EntityNotFoundException("User");
        }
        
        var verificationResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!verificationResult.Succeeded)
        {
            throw new AuthenticationException("Email or password is incorrect");
        }
        
        var client = _httpClientFactory.CreateClient();
        var discoveryDocument = await client.GetDiscoveryDocumentAsync(_identityServerUri);
        if (discoveryDocument.IsError)
        {
            throw new AuthenticationException("Identity server error");
        }

        var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = discoveryDocument.TokenEndpoint,
            GrantType = GrantType.ResourceOwnerPassword,
            ClientId = _clientId,
            ClientSecret = _clientSecret,
            UserName = user.UserName,
            Password = model.Password,
            Scope = "api offline_access"
        });

        if (tokenResponse.IsError)
        {
            throw new AuthenticationException("Identity server error");
        }
        
        return new TokenResponce
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken
        };
    }
}