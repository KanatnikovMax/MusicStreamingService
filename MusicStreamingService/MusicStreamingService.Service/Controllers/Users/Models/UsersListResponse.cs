using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.Service.Controllers.Users.Models;

public record UsersListResponse(List<User> Users);