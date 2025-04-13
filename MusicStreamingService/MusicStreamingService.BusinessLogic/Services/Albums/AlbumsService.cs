using AutoMapper;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.Repositories.Interfaces;

namespace MusicStreamingService.BusinessLogic.Services.Albums;

public class AlbumsService : IAlbumsService
{
    private readonly IAlbumsRepository _albumsRepository;
    private readonly IArtistsRepository _artistsRepository;
    private readonly IMapper _mapper;

    public AlbumsService(IAlbumsRepository albumsRepository, IArtistsRepository artistsRepository, IMapper mapper)
    {
        _albumsRepository = albumsRepository;
        _artistsRepository = artistsRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<AlbumModel>> GetAllAlbumsAsync()
    {
        var albums = await _albumsRepository.FindAllAsync();
        return _mapper.Map<IEnumerable<AlbumModel>>(albums);
    }

    public async Task<AlbumModel> GetAlbumByIdAsync(Guid id)
    {
        var album = await _albumsRepository.FindByIdAsync(id)
            ?? throw new EntityNotFoundException("Album", id);
        return _mapper.Map<AlbumModel>(album);
    }

    public async Task<IEnumerable<AlbumModel>> GetAlbumByNameAsync(string titlePart)
    {
        var albums = await _albumsRepository.FindByTitlePartAsync(titlePart);
        return _mapper.Map<IEnumerable<AlbumModel>>(albums);
    }

    public async Task<AlbumModel> CreateAlbumAsync(CreateAlbumModel model)
    {
        var album = _mapper.Map<Album>(model);
        album = await _albumsRepository.SaveAsync(album, model.Artists)
            ?? throw new EntityAlreadyExistsException("Album");
        
        return _mapper.Map<AlbumModel>(album);
    }

    public async Task<AlbumModel> DeleteAlbumAsync(Guid id)
    {
        var album = await _albumsRepository.FindByIdAsync(id)
                     ?? throw new EntityNotFoundException("Album", id);
        
        await _albumsRepository.DeleteAsync(album);
        return _mapper.Map<AlbumModel>(album);
    }

    public async Task<AlbumModel> UpdateAlbumAsync(UpdateAlbumModel model, Guid id) 
    {
        var album = await _albumsRepository.FindByIdAsync(id)
                    ?? throw new EntityNotFoundException("Album", id);
        album.Title = model.Title ?? album.Title;
        album.ReleaseDate = model.ReleaseDate ?? album.ReleaseDate;
        if (model.Artists is not null)
        {
            var artists = new List<Artist>();
            foreach (var name in model.Artists)
            {
                var artist = await _artistsRepository.FindByNameAsync(name) 
                             ?? await _artistsRepository.SaveAsync(new Artist { Name = name });
                artists.Add(artist!);
            }
            album.Artists = artists;
        }

        album = await _albumsRepository.UpdateAsync(album);
        return _mapper.Map<AlbumModel>(album);
    }
}