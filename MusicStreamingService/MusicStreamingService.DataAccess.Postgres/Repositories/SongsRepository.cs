using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Postgres.Context;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.Postgres.Repositories;

public class SongsRepository : ISongsRepository
{
    private readonly MusicServiceDbContext _context;

    public SongsRepository(MusicServiceDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<CursorResponse<PopularityCursor?, Song>> FindAllAsync(PaginationParams<PopularityCursor> request)
    {
        var songs = _context.Set<Song>()
            .Include(s => s.Artists)
            .AsNoTracking();
        
        if (request.Cursor is not null)
        {
            songs = songs.Where(s => s.PlayCount < request.Cursor.PlayCount
                                     || (s.PlayCount == request.Cursor.PlayCount
                                         && s.CreatedAt > request.Cursor.CreatedAt));
        }

        var items = await songs
            .OrderByDescending(s => s.PlayCount)
            .ThenBy(s => s.CreatedAt)
            .Take(request.PageSize + 1)
            .ToListAsync();
        
        var cursorItem = items.Count > request.PageSize ? items[request.PageSize - 1] : null;
        var cursor = cursorItem is not null
            ? new PopularityCursor(cursorItem.PlayCount, cursorItem.CreatedAt)
            : null;

        return new CursorResponse<PopularityCursor?, Song>
        {
            Cursor = cursor,
            Items = items.Take(request.PageSize).ToList()
        };
    }

    public async Task<IEnumerable<Song>> FindAllAsync(Expression<Func<Song, bool>> predicate)
    {
        return await _context.Set<Song>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Song?> FindByIdAsync(Guid id)
    {
        return await _context.Set<Song>()
            .Include(s => s.Artists)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public void Delete(Song entity)
    {
        _context.Set<Song>().Remove(entity);
    }

    public async Task<Song?> SaveAsync(Song entity)
    {
        var song = _context.Set<Song>().FirstOrDefault(a => a.Id == entity.Id);
        
        if (song is not null) 
            return null;
        
        var result = await _context.Set<Song>().AddAsync(entity);
        return result.Entity;
    }

    public Song Update(Song entity)
    {
        var result = _context.Set<Song>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return result.Entity;
    }
    
    public async Task<Song?> FindByTitleAsync(string title)
    {
        return await _context.Set<Song>()
            .Include(s => s.Artists)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => EF.Functions.ILike(a.Title, title));
    }

    public async Task<List<Song>> FindByIdsAsync(IEnumerable<Guid> ids)
    {
        var songIds = ids.Distinct().ToList();
        return await _context.Set<Song>()
            .Include(s => s.Artists)
            .AsNoTracking()
            .Where(s => songIds.Contains(s.Id))
            .ToListAsync();
    }
    
    public async Task<CursorResponse<PopularityCursor?, Song>> FindByTitlePartAsync(string titlePart, 
        PaginationParams<PopularityCursor> request)
    {
        var songs = _context.Set<Song>()
            .Include(s => s.Artists)
            .AsNoTracking()
            .Where(s => EF.Functions.TrigramsAreSimilar(s.Title, titlePart));
        
        if (request.Cursor is not null)
        {
            songs = songs.Where(s => s.PlayCount < request.Cursor.PlayCount
                                     || (s.PlayCount == request.Cursor.PlayCount
                                         && s.CreatedAt > request.Cursor.CreatedAt));
        }

        var items = await songs
            .OrderByDescending(s => s.PlayCount)
            .ThenBy(s => s.CreatedAt)
            .Take(request.PageSize + 1)
            .ToListAsync();
        
        var cursorItem = items.Count > request.PageSize ? items[request.PageSize - 1] : null;
        var cursor = cursorItem is not null
            ? new PopularityCursor(cursorItem.PlayCount, cursorItem.CreatedAt)
            : null;
        
        return new CursorResponse<PopularityCursor?, Song>
        {
            Cursor = cursor,
            Items = items.Take(request.PageSize).ToList()
        };
    }

    public async Task<List<Guid>?> IncrementPlayCountAsync(Guid songId)
    {
        var artistIds = await _context.Set<ArtistSong>()
            .Where(artistSong => artistSong.SongId == songId)
            .Select(artistSong => artistSong.ArtistId)
            .ToListAsync();

        if (artistIds.Count == 0 && !await _context.Set<Song>().AnyAsync(song => song.Id == songId))
        {
            return null;
        }

        await _context.Set<Song>()
            .Where(song => song.Id == songId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(song => song.PlayCount, song => song.PlayCount + 1));

        if (artistIds.Count > 0)
        {
            await _context.Set<Artist>()
                .Where(artist => artistIds.Contains(artist.Id))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(artist => artist.PlayCount, artist => artist.PlayCount + 1));
        }

        return artistIds;
    }
}
