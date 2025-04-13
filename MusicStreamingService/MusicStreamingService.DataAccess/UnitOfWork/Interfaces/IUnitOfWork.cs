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
    IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);
    Task CommitAsync();
    void Rollback();
}