using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Repositories.Interfaces;
using MusicStreamingService.DataAccess.UnitOfWork.Interfaces;

namespace MusicStreamingService.DataAccess.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly MusicServiceDbContext _context;
    public IArtistsRepository Artists { get; }
    public IAlbumsRepository Albums { get; }
    public ISongsRepository Songs { get; }
    public IUsersRepository Users { get; }

    public UnitOfWork(MusicServiceDbContext context, 
        IArtistsRepository artists, IAlbumsRepository albums, ISongsRepository songs, IUsersRepository users)
    {
        _context = context;
        Artists = artists;
        Albums = albums;
        Songs = songs;
        Users = users;
    }

    public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        return _context.Database.BeginTransaction(isolationLevel);
    }

    public async Task CommitAsync()
    {
       await _context.SaveChangesAsync();
    }

    public void Rollback()
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