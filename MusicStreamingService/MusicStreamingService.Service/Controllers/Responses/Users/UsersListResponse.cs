using MusicStreamingService.BusinessLogic.Services.Users.Models;

namespace MusicStreamingService.Service.Controllers.Responses.Users;

public record UsersListResponse(List<UserModel> Users);