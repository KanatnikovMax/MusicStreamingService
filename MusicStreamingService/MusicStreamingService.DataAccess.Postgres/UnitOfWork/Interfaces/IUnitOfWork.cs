using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.Postgres.UnitOfWork.Interfaces;

public interface IUnitOfWork
{
    IArtistsRepository Artists { get; }
    IAlbumsRepository Albums { get; }
    ISongsRepository Songs { get; }
    IUsersRepository Users { get; }
    Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    Task CommitAsync();
    Task RollbackAsync();
}