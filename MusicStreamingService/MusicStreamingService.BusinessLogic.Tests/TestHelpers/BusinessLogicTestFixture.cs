using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using MusicStreamingService.BusinessLogic.Mapper;
using MusicStreamingService.DataAccess.Postgres.Repositories.Interfaces;
using MusicStreamingService.DataAccess.Postgres.UnitOfWork.Interfaces;
using MusicStreamingService.MediaLibrary;

namespace MusicStreamingService.BusinessLogic.Tests.TestHelpers;

public sealed class BusinessLogicTestFixture
{
    public BusinessLogicTestFixture()
    {
        SetupFixture();

        Mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AlbumsBLProfile>();
            cfg.AddProfile<ArtistsBLProfile>();
            cfg.AddProfile<SongsBLProfile>();
            cfg.AddProfile<PlaylistsBLProfile>();
            cfg.AddProfile<UsersBLProfile>();
        }).CreateMapper();
        
        UnitOfWork.SetupGet(x => x.Albums).Returns(Albums.Object);
        UnitOfWork.SetupGet(x => x.Artists).Returns(Artists.Object);
        UnitOfWork.SetupGet(x => x.Songs).Returns(Songs.Object);
        UnitOfWork.SetupGet(x => x.Playlists).Returns(Playlists.Object);
        UnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<System.Data.IsolationLevel>()))
            .ReturnsAsync(Mock.Of<IDbContextTransaction>());
    }

    public IFixture Fixture { get; private set; } = null!;
    public IMapper Mapper { get; private set; }
    public Mock<IUnitOfWork> UnitOfWork { get; } = new();
    public Mock<IAlbumsRepository> Albums { get; } = new();
    public Mock<IArtistsRepository> Artists { get; } = new();
    public Mock<ISongsRepository> Songs { get; } = new();
    public Mock<IPlaylistsRepository> Playlists { get; } = new();
    public Mock<IDistributedCache> Cache { get; } = new();
    public Mock<IMediaStorageService> MediaStorage { get; } = new();
    
    private void SetupFixture()
    {
        Fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        Fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => Fixture.Behaviors.Remove(b));

        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }
}
