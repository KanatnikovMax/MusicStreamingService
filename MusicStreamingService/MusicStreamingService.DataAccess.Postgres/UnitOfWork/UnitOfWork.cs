using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MusicStreamingService.DataAccess.Postgres.Context;
using MusicStreamingService.DataAccess.Postgres.Repositories;
using MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;
using MusicStreamingService.DataAccess.Postgres.UnitOfWork.Interfaces;

namespace MusicStreamingService.DataAccess.Postgres.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly MusicServiceDbContext _context;
    private IDbContextTransaction? _transaction;
    public IArtistsRepository Artists { get; }
    public IAlbumsRepository Albums { get; }
    public ISongsRepository Songs { get; }
    public IUsersRepository Users { get; }

    public UnitOfWork(MusicServiceDbContext context)
    {
        _context = context;
        _transaction = null;
        Artists = new ArtistsRepository(context);
        Albums = new AlbumsRepository(context);
        Songs = new SongsRepository(context);
        Users = new UsersRepository(context);
    }
    
    public async Task<IDbContextTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel )
    {
        _transaction = await _context.Database.BeginTransactionAsync(isolationLevel);
        return _transaction;
    }

    public async Task CommitAsync()
    {
       await _context.SaveChangesAsync();
       if (_transaction is not null)
           await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }
        
        _transaction = null;
        DetachAllEntities();
    }

    private void DetachAllEntities()
    {
        foreach (var entry in _context.ChangeTracker.Entries())
        {
            entry.State = EntityState.Detached;
        }
    }

    private bool _disposed = false;
 
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }
    }
 
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}