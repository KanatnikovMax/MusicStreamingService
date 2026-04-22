using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Postgres.Context;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.Postgres.Repositories;

public class PlaylistsRepository : IPlaylistsRepository
{
    private readonly MusicServiceDbContext _context;

    public PlaylistsRepository(MusicServiceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Playlist>> FindAllAsync()
    {
        return await _context.Set<Playlist>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Playlist>> FindAllAsync(Expression<Func<Playlist, bool>> predicate)
    {
        return await _context.Set<Playlist>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Playlist?> FindByIdAsync(Guid id)
    {
        return await _context.Set<Playlist>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Playlist?> FindUserPlaylistByIdAsync(Guid userId, Guid playlistId)
    {
        return await _context.Set<Playlist>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == playlistId && p.UserId == userId);
    }

    public async Task<CursorResponse<DateTime?, Playlist>> FindAllByUserIdAsync(Guid userId, string? namePart,
        PaginationParams<DateTime?> request)
    {
        var playlists = _context.Set<Playlist>()
            .Where(p => p.UserId == userId)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(namePart))
        {
            playlists = playlists.Where(p => EF.Functions.ILike(p.Name, $"%{namePart}%"));
        }

        if (request.Cursor is not null)
        {
            playlists = playlists.Where(p => p.CreatedAt <= request.Cursor);
        }

        var items = await playlists
            .OrderByDescending(p => p.CreatedAt)
            .Take(request.PageSize + 1)
            .ToListAsync();

        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.CreatedAt : null;

        return new CursorResponse<DateTime?, Playlist>
        {
            Cursor = cursor,
            Items = items.Take(request.PageSize).ToList()
        };
    }

    public async Task<CursorResponse<int?, Song>> FindSongsAsync(Guid playlistId, string? namePart,
        PaginationParams<int?> request)
    {
        var playlistSongs = _context.Set<PlaylistSong>()
            .Where(ps => ps.PlaylistId == playlistId)
            .Include(ps => ps.Song)
            .ThenInclude(s => s.Artists)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(namePart))
        {
            playlistSongs = playlistSongs.Where(ps =>
                EF.Functions.ILike(ps.Song.Title, $"%{namePart}%") ||
                ps.Song.Artists.Any(a => EF.Functions.ILike(a.Name, $"%{namePart}%")));
        }

        if (request.Cursor is not null)
        {
            playlistSongs = playlistSongs.Where(ps => ps.Order >= request.Cursor);
        }

        var items = await playlistSongs
            .OrderBy(ps => ps.Order)
            .Take(request.PageSize + 1)
            .ToListAsync();

        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.Order : null;

        return new CursorResponse<int?, Song>
        {
            Cursor = cursor,
            Items = items.Take(request.PageSize)
                .Select(ps => ps.Song)
                .ToList()
        };
    }

    public void Delete(Playlist entity)
    {
        _context.Set<Playlist>().Remove(entity);
    }

    public async Task<Playlist?> SaveAsync(Playlist entity)
    {
        var playlist = _context.Set<Playlist>().FirstOrDefault(p => p.Id == entity.Id);

        if (playlist is not null)
        {
            return null;
        }
        
        var result = await _context.Set<Playlist>().AddAsync(entity);
        return result.Entity;
    }

    public Playlist Update(Playlist entity)
    {
        var result = _context.Set<Playlist>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return result.Entity;
    }

    public async Task<PlaylistSong?> AddSongAsync(Guid playlistId, Guid songId)
    {
        var existingSong = await _context.Set<PlaylistSong>()
            .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

        if (existingSong is not null)
        {
            return null;
        }
        
        var maxOrder = await _context.Set<PlaylistSong>()
            .Where(ps => ps.PlaylistId == playlistId)
            .Select(ps => (int?)ps.Order)
            .MaxAsync() ?? 0;

        var playlistSong = new PlaylistSong
        {
            PlaylistId = playlistId,
            SongId = songId,
            AddedAt = DateTime.UtcNow,
            Order = maxOrder + 1
        };

        await _context.Set<PlaylistSong>().AddAsync(playlistSong);
        return playlistSong;
    }

    public async Task<PlaylistSong?> FindSongAsync(Guid playlistId, Guid songId)
    {
        return await _context.Set<PlaylistSong>()
            .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
    }

    public void DeleteSong(PlaylistSong playlistSong)
    {
        _context.Set<PlaylistSong>().Remove(playlistSong);
    }
}
