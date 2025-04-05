using MusicStreamingService.BusinessLogic.Services.Users.Models;

namespace MusicStreamingService.Service.Controllers.Users.Models;

public record UsersListResponse(List<UserModel> Users);