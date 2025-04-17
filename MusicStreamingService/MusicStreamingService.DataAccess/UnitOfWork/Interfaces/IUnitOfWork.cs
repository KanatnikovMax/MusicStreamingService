using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using MusicStreamingService.DataAccess.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.UnitOfWork.Interfaces;

public interface IUnitOfWork
{
    IArtistsRepository Artists { get; }
    IAlbumsRepository Albums { get; }
    ISongsRepository Songs { get; }
    IUsersRepository Users { get; }
    Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel);
    Task CommitAsync();
    Task RollbackAsync();
}