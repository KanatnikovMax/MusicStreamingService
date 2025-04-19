using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.Repositories;

public class ArtistsRepository : IArtistsRepository 
{
    private readonly MusicServiceDbContext _context;

    public ArtistsRepository(MusicServiceDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<PaginatedResponse<Artist>> FindAllAsync(PaginationParams request)
    {
        var artists = _context.Set<Artist>()
            .Include(a => a.Albums)
            .Include(a => a.Songs)
            .AsNoTracking();
        
        if (request.Cursor is not null)
        {
            artists = artists.Where(s => s.CreatedAt >= request.Cursor);
        }

        var items = await artists.OrderBy(s => s.CreatedAt)
            .Take(request.PageSize + 1)
            .ToListAsync();
        
        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.CreatedAt : null;
        
        return new PaginatedResponse<Artist>
        {
            Cursor = cursor,
            Items = items.Take(request.PageSize).ToList(),
        };
    }

    public async Task<IEnumerable<Artist>> FindAllAsync(Expression<Func<Artist, bool>> predicate)
    {
        return await _context.Set<Artist>()
            .Include(a => a.Albums)
            .Where(predicate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Album>> FindAllAlbumsAsync(Guid artistId)
    {
        var artist = await _context.Set<Artist>()
            .Include(a => a.Albums)!
            .ThenInclude(a => a.Artists)
            .FirstOrDefaultAsync(a => a.Id == artistId);
        
        if (artist is null)
            return Enumerable.Empty<Album>();
        
        return artist.Albums?.OrderByDescending(a => a.ReleaseDate)
               ?? Enumerable.Empty<Album>();
    }

    public async Task<IEnumerable<Song>> FindAllSongsAsync(Guid artistId)
    {
        var artist = await _context.Set<Artist>()
            .Include(a => a.Songs)!
            .ThenInclude(s => s.Artists)
            .FirstOrDefaultAsync(a => a.Id == artistId);
        
        return artist?.Songs
               ?? Enumerable.Empty<Song>();
    }

    public async Task<Artist?> FindByIdAsync(Guid id)
    {
        return await _context.Set<Artist>()
            .Include(a => a.Albums)! 
            .ThenInclude(a => a.Artists) 
            .Include(a => a.Songs)!
            .ThenInclude(s => s.Artists)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
    
    public async Task<Artist?> FindByNameAsync(string name)
    {
        return await _context.Set<Artist>()
            .Include(a => a.Albums)! 
            .ThenInclude(a => a.Artists) 
            .Include(a => a.Songs)!
            .ThenInclude(s => s.Artists)
            .FirstOrDefaultAsync(a => EF.Functions.ILike(a.Name, name));
   }
    
    public async Task<PaginatedResponse<Artist>> FindByNamePartAsync(string namePart, PaginationParams request)
    {
        var artists = _context.Set<Artist>()
            .Include(a => a.Albums)! 
            .ThenInclude(a => a.Artists) 
            .Include(a => a.Songs)!
            .ThenInclude(s => s.Artists)
            .Where(a => EF.Functions.ILike(a.Name, $"%{namePart}%")) ;
        
        if (request.Cursor is not null)
        {
            artists = artists.Where(s => s.CreatedAt >= request.Cursor);
        }

        var items = await artists.OrderBy(s => s.CreatedAt)
            .Take(request.PageSize + 1)
            .ToListAsync();
        
        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.CreatedAt : null;
        
        return new PaginatedResponse<Artist>
        {
            Cursor = cursor,
            Items = items.Take(request.PageSize).ToList(),
        };
    }

    public void Delete(Artist entity)
    {
        _context.Set<Artist>().Remove(entity);
    }

    public async Task<Artist?> SaveAsync(Artist entity)
    {
        var artist = _context.Set<Artist>().FirstOrDefault(a => a.Id == entity.Id 
                                                               || a.Name.ToLower() == entity.Name.ToLower());
        if (artist is not null)
        {
            return null;
        }
        
        var result = await _context.Set<Artist>().AddAsync(entity);
        return result.Entity;
    }

    public Artist Update(Artist entity)
    {
        var result = _context.Set<Artist>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return result.Entity;
    }

    public async Task<PaginatedResponse<Album>> FindAllAlbumsAsync(Guid artistId, PaginationParams request)
    {
        var artist = await _context.Set<Artist>()
            .Include(a => a.Albums)!
            .ThenInclude(a => a.Artists)
            .FirstOrDefaultAsync(a => a.Id == artistId);

        if (artist is null)
        {
            return new PaginatedResponse<Album>
            {
                Cursor = null,
                Items = []
            };
        }
        
        var albums =  artist.Albums?.OrderByDescending(a => a.ReleaseDate)
            ?? Enumerable.Empty<Album>();
        
        if (request.Cursor is not null)
        {
            albums = albums.Where(s => s.CreatedAt >= request.Cursor);
        }

        var items = albums.OrderBy(s => s.CreatedAt)
            .Take(request.PageSize + 1)
            .ToList();
        
        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.CreatedAt : null;
        
        return new PaginatedResponse<Album>
        {
            Cursor = cursor,
            Items = items.Take(request.PageSize).ToList(),
        };
    }

    public async Task<PaginatedResponse<Song>> FindAllSongsAsync(Guid artistId, PaginationParams request)
    {
        var artist = await _context.Set<Artist>()
            .Include(a => a.Songs)!
            .ThenInclude(s => s.Artists)
            .FirstOrDefaultAsync(a => a.Id == artistId);

        if (artist is null)
        {
            return new PaginatedResponse<Song>
            {
                Cursor = null,
                Items = []
            };
        }

        var songs = artist.Songs ?? [];
        
        if (request.Cursor is not null)
        {
            songs = songs.Where(s => s.CreatedAt >= request.Cursor).ToList();
        }

        var items = songs.OrderBy(s => s.CreatedAt)
            .Take(request.PageSize + 1)
            .ToList();
        
        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.CreatedAt : null;
        
        return new PaginatedResponse<Song>
        {
            Cursor = cursor,
            Items = items.Take(request.PageSize).ToList(),
        };
    }

    public async Task<PaginatedResponse<Song>> FindAllSongsByTitleAsync(Guid artistId, string titlePart, 
        PaginationParams request)
    {
        var artist = await _context.Set<Artist>()
            .Include(a => a.Songs)!
            .ThenInclude(s => s.Artists)
            .FirstOrDefaultAsync(a => a.Id == artistId);

        if (artist is null)
        {
            return new PaginatedResponse<Song>
            {
                Cursor = null,
                Items = []
            };
        }

        var songs = artist.Songs ?? [];
        
        if (request.Cursor is not null)
        {
            songs = songs.Where(s => s.Title.IndexOf(titlePart, StringComparison.OrdinalIgnoreCase) >= 0
                    && s.CreatedAt >= request.Cursor).ToList();
        }

        var items = songs
            .OrderBy(s => s.CreatedAt)
            .Take(request.PageSize + 1)
            .ToList();
        
        var cursor = items.Count > request.PageSize ? items.LastOrDefault()?.CreatedAt : null;
        
        return new PaginatedResponse<Song>
        {
            Cursor = cursor,
            Items = items.Take(request.PageSize).ToList(),
        };
    }
    
    public async Task<List<Artist>> GetOrCreateArtistsAsync(IEnumerable<string> names)
    {
        var nameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in names)
        {
            var trimmedName = name.Trim();
            var normalized = trimmedName.ToLowerInvariant();
            nameMap.TryAdd(normalized, trimmedName);
        }
        var normalizedNames = nameMap.Keys.ToList();

        var existingArtists = await _context.Set<Artist>()
            .Where(a => normalizedNames.Contains(a.Name.ToLower()))
            .ToListAsync();

        var existingNames = new HashSet<string>(
            existingArtists.Select(a => a.Name.ToLower()), 
            StringComparer.OrdinalIgnoreCase
        );

        var newArtists = normalizedNames
            .Where(n => !existingNames.Contains(n))
            .Select(n => new Artist { Name = nameMap[n] }) 
            .ToList();

        if (newArtists.Count > 0)
        {
            await _context.Set<Artist>().AddRangeAsync(newArtists);
        }

        return existingArtists.Concat(newArtists).ToList();
    }
}