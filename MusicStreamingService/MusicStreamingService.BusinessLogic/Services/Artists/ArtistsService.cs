using System.Data;
using AutoMapper;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
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

    public async Task<IEnumerable<ArtistModel>> GetAllArtistsAsync()
    {
        var artists = await _unitOfWork.Artists.FindAllAsync();
        return _mapper.Map<IEnumerable<ArtistModel>>(artists);
    }

    public async Task<ArtistModel> GetArtistByIdAsync(Guid id)
    {
        var artist = await _unitOfWork.Artists.FindByIdAsync(id)
            ?? throw new EntityNotFoundException("Artist", id);
        return _mapper.Map<ArtistModel>(artist);
    }

    public async Task<IEnumerable<ArtistModel>> GetArtistByNameAsync(string namePart)
    {
        var artists = await _unitOfWork.Artists.FindByNamePartAsync(namePart);
        return _mapper.Map<IEnumerable<ArtistModel>>(artists);
    }

    public async Task<IEnumerable<AlbumModel>> GetAllAlbumsAsync(Guid artistId)
    {
        var albums = await _unitOfWork.Artists.FindAllAlbumsAsync(artistId);
        return _mapper.Map<IEnumerable<AlbumModel>>(albums);
    }

    public async Task<ArtistModel> CreateArtistAsync(CreateArtistModel model)
    {
        var artist = _mapper.Map<Artist>(model);
        await using var transaction = _unitOfWork.BeginTransaction(IsolationLevel.Serializable);
        try
        {
            artist = await _unitOfWork.Artists.SaveAsync(artist);
            if (artist is null)
            {
                await transaction.RollbackAsync();
                throw new EntityAlreadyExistsException("Artist");
            }
            
            await _unitOfWork.CommitAsync();
            await transaction.CommitAsync();
            
            return _mapper.Map<ArtistModel>(artist);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ArtistModel> DeleteArtistAsync(Guid id)
    {
        var artist = await _unitOfWork.Artists.FindByIdAsync(id)
            ?? throw new EntityNotFoundException("Artist", id);
        
        _unitOfWork.Artists.Delete(artist);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<ArtistModel>(artist);
    }

    public async Task<ArtistModel> UpdateArtistAsync(UpdateArtistModel model, Guid id)
    {
        var artist = await _unitOfWork.Artists.FindByIdAsync(id)
                     ?? throw new EntityNotFoundException("Artist", id);
        artist.Name = model.Name ?? artist.Name;
        artist = _unitOfWork.Artists.Update(artist);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<ArtistModel>(artist);
    }
}