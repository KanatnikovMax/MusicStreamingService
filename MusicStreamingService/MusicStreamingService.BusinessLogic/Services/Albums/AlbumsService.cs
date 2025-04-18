using System.Data;
using AutoMapper;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.UnitOfWork.Interfaces;

namespace MusicStreamingService.BusinessLogic.Services.Albums;

public class AlbumsService : IAlbumsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AlbumsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AlbumModel>> GetAllAlbumsAsync()
    {
        var albums = await _unitOfWork.Albums.FindAllAsync();
        return _mapper.Map<IEnumerable<AlbumModel>>(albums);
    }

    public async Task<AlbumModel> GetAlbumByIdAsync(Guid id)
    {
        var album = await _unitOfWork.Albums.FindByIdAsync(id)
            ?? throw new EntityNotFoundException("Album", id);
        return _mapper.Map<AlbumModel>(album);
    }

    public async Task<IEnumerable<AlbumModel>> GetAlbumByNameAsync(string titlePart)
    {
        var albums = await _unitOfWork.Albums.FindByTitlePartAsync(titlePart);
        return _mapper.Map<IEnumerable<AlbumModel>>(albums);
    }

    public async Task<IEnumerable<SongModel>> GetAllAlbumSongsAsync(Guid albumId)
    {
        var songs = await _unitOfWork.Albums.FindAllSongsAsync(albumId);
        
        return _mapper.Map<IEnumerable<SongModel>>(songs);
    }
    
    public async Task<AlbumModel> CreateAlbumAsync(CreateAlbumModel model)
    {
        var entity = _mapper.Map<Album>(model);
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var album = await _unitOfWork.Albums.FindByTitleAsync(entity.Title);
            if (album is not null)
            {
                var artistsNames = album.Artists
                    .Select(a => a.Name.ToLower())
                    .OrderBy(name => name)
                    .ToList();

                var existingArtistsNames = model.Artists
                    .Select(n => n.ToLower())
                    .OrderBy(name => name)
                    .ToList();

                var isDuplicate = artistsNames
                    .SequenceEqual(existingArtistsNames);

                if (isDuplicate)
                {
                    await _unitOfWork.RollbackAsync();
                    throw new EntityAlreadyExistsException("Album");
                }
            }
        
            var artists = await _unitOfWork.Artists.GetOrCreateArtistsAsync(model.Artists);
            entity.Artists = artists;
            entity = await _unitOfWork.Albums.SaveAsync(entity);
            if (entity is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityAlreadyExistsException("Album");
            }
            await _unitOfWork.CommitAsync();
        
            return _mapper.Map<AlbumModel>(entity);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<AlbumModel> DeleteAlbumAsync(Guid id)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var album = await _unitOfWork.Albums.FindByIdAsync(id);
            if (album is null)
            {
                await _unitOfWork.RollbackAsync();
                throw new EntityNotFoundException("Album", id);
            }

            _unitOfWork.Albums.Delete(album);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<AlbumModel>(album);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<AlbumModel> UpdateAlbumAsync(UpdateAlbumModel model, Guid id)
    {
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        try
        {
            var album = await _unitOfWork.Albums.FindByIdAsync(id)
                        ?? throw new EntityNotFoundException("Album", id);

            if (!string.IsNullOrEmpty(model.Title))
            {
                album.Title = model.Title;
            }
            if (model.ReleaseDate.HasValue)
            {
                album.ReleaseDate = model.ReleaseDate.Value;
            }
            if (model.Artists != null)
            {
                var artists = await _unitOfWork.Artists.GetOrCreateArtistsAsync(model.Artists);
                
                album.Artists.Clear();
                foreach (var artist in artists)
                {
                    album.Artists.Add(artist);
                }
            }
            _unitOfWork.Albums.Update(album);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<AlbumModel>(album);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}