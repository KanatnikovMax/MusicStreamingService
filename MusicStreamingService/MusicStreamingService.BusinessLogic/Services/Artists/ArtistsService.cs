using AutoMapper;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.DataAccess.Entities;
using MusicStreamingService.DataAccess.Repositories.Interfaces;

namespace MusicStreamingService.BusinessLogic.Services.Artists;

public class ArtistsService : IArtistsService
{
    private readonly IArtistsRepository _artistsRepository;
    private readonly IMapper _mapper;

    public ArtistsService(IArtistsRepository artistsRepository, IMapper mapper)
    {
        _artistsRepository = artistsRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ArtistModel>> GetAllArtistsAsync()
    {
        var artists = await _artistsRepository.FindAllAsync();
        return _mapper.Map<IEnumerable<ArtistModel>>(artists);
    }

    public async Task<ArtistModel> GetArtistByIdAsync(Guid id)
    {
        var artist = await _artistsRepository.FindByIdAsync(id)
            ?? throw new EntityNotFoundException("Artist", id);
        
        return _mapper.Map<ArtistModel>(artist);
    }

    public async Task<IEnumerable<ArtistModel>> GetArtistByNameAsync(string namePart)
    {
        var artists = await _artistsRepository.FindByNamePartAsync(namePart);
        return _mapper.Map<IEnumerable<ArtistModel>>(artists);
    }

    public async Task<IEnumerable<AlbumModel>> GetAllAlbumsAsync(Guid artistId)
    {
        var albums = await _artistsRepository.FindAllAlbumsAsync(artistId);
        return _mapper.Map<IEnumerable<AlbumModel>>(albums);
    }

    public async Task<ArtistModel> CreateArtistAsync(CreateArtistModel model)
    {
        var artist = _mapper.Map<Artist>(model);
        artist = await _artistsRepository.SaveAsync(artist)
            ?? throw new EntityAlreadyExistsException("Artist");
        
        return _mapper.Map<ArtistModel>(artist);
    }

    public async Task<ArtistModel> DeleteArtistAsync(Guid id)
    {
        var artist = await _artistsRepository.FindByIdAsync(id)
            ?? throw new EntityNotFoundException("Artist", id);
        
        await _artistsRepository.DeleteAsync(artist);
        return _mapper.Map<ArtistModel>(artist);
    }

    public async Task<ArtistModel> UpdateArtistAsync(UpdateArtistModel model, Guid id)
    {
        var artist = await _artistsRepository.FindByIdAsync(id)
                     ?? throw new EntityNotFoundException("Artist", id);
        artist.Name = model.Name ?? artist.Name;
        artist = await _artistsRepository.UpdateAsync(artist);
        return _mapper.Map<ArtistModel>(artist);
    }
}