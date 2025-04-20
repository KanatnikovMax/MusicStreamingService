using System.Data;
using AutoMapper;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.UnitOfWork.Interfaces;

namespace MusicStreamingService.BusinessLogic.Services.Artists;

public class ArtistsService : IArtistsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ArtistsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<DateTime?, ArtistModel>> GetAllArtistsAsync(PaginationParams<DateTime?> request)
    {
        var artists = await _unitOfWork.Artists.FindAllAsync(request);
        return new PaginatedResponse<DateTime?, ArtistModel>
        {
            Cursor = artists.Cursor,
            Items = _mapper.Map<List<ArtistModel>>(artists.Items)
        };
    }

    public async Task<ArtistModel> GetArtistByIdAsync(Guid id)
    {
        var artist = await _unitOfWork.Artists.FindByIdAsync(id)
            ?? throw new EntityNotFoundException("Artist", id);
        return _mapper.Map<ArtistModel>(artist);
    }

    public async Task<PaginatedResponse<DateTime?, ArtistModel>> GetArtistByNameAsync(string namePart, 
        PaginationParams<DateTime?> request)
    {
        var artists = await _unitOfWork.Artists.FindByNamePartAsync(namePart, request);
        return new PaginatedResponse<DateTime?, ArtistModel>
        {
            Cursor = artists.Cursor,
            Items = _mapper.Map<List<ArtistModel>>(artists.Items)
        };
    }

    public async Task<PaginatedResponse<DateTime?, AlbumModel>> GetAllAlbumsAsync(Guid artistId, 
        PaginationParams<DateTime?> request)
    {
        var albums = await _unitOfWork.Artists.FindAllAlbumsAsync(artistId, request);
        return new PaginatedResponse<DateTime?, AlbumModel>
        {
            Cursor = albums.Cursor,
            Items = _mapper.Map<List<AlbumModel>>(albums.Items)
        };
    }
    
    public async Task<PaginatedResponse<DateTime?, SongModel>> GetAllSongsAsync(Guid artistId, 
        PaginationParams<DateTime?> request)
    {
        var songs = await _unitOfWork.Artists.FindAllSongsAsync(artistId, request);
        return new PaginatedResponse<DateTime?, SongModel>
        {
            Cursor = songs.Cursor,
            Items = _mapper.Map<List<SongModel>>(songs.Items)
        };
    }
    
    public async Task<PaginatedResponse<DateTime?, SongModel>> GetSongsByTitleAsync(Guid artistId, string titlePart,
        PaginationParams<DateTime?> request)
    {
        var songs = await _unitOfWork.Artists.FindAllSongsByTitleAsync(artistId, titlePart, request);
        return new PaginatedResponse<DateTime?, SongModel>
        {
            Cursor = songs.Cursor,
            Items = _mapper.Map<List<SongModel>>(songs.Items)
        };
    }

    public async Task<ArtistModel> CreateArtistAsync(CreateArtistModel model)
    {
        var artist = _mapper.Map<Artist>(model);
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            artist = await _unitOfWork.Artists.SaveAsync(artist);
            if (artist is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityAlreadyExistsException("Artist");
            }
            
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ArtistModel>(artist);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<ArtistModel> DeleteArtistAsync(Guid id)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var artist = await _unitOfWork.Artists.FindByIdAsync(id);
            if (artist is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityNotFoundException("Artist", id);
            }

            var albums = await _unitOfWork.Artists.FindAllAlbumsAsync(id);
            var albumsToDelete = albums.Where(a => a.Artists.Count == 1).ToList();

            foreach (var album in albumsToDelete)
            {
                _unitOfWork.Albums.Delete(album);
            }

            _unitOfWork.Artists.Delete(artist);
        
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ArtistModel>(artist);
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<ArtistModel> UpdateArtistAsync(UpdateArtistModel model, Guid id)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var artist = await _unitOfWork.Artists.FindByIdAsync(id);
            if (artist is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityNotFoundException("Artist", id);
            }
            artist.Name = model.Name ?? artist.Name;
            artist = _unitOfWork.Artists.Update(artist);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ArtistModel>(artist);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
        
    }
}