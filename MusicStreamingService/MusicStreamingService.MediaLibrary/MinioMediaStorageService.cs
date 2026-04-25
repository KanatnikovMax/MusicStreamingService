using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using MusicStreamingService.BusinessLogic.Services.Media.Models;
using MusicStreamingService.MediaLibrary.Settings;

namespace MusicStreamingService.MediaLibrary;

public class MinioMediaStorageService : IMediaStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _settings;
    private readonly ILogger<MinioMediaStorageService> _logger;
    private readonly IDistributedCache _cache;
    private readonly SemaphoreSlim _bucketInitializationLock = new(1, 1);
    private bool _bucketInitialized;

    public MinioMediaStorageService(
        IMinioClient minioClient,
        MinioSettings settings,
        IDistributedCache cache,
        ILogger<MinioMediaStorageService> logger)
    {
        _minioClient = minioClient;
        _settings = settings;
        _cache = cache;
        _logger = logger;
    }

    public async Task<string?> UploadAsync(
        FileUploadModel? file,
        string entityType,
        Guid entityId,
        CancellationToken cancellationToken = default)
    {
        if (file is null)
        {
            return null;
        }

        await EnsureBucketExistsAsync(cancellationToken);

        var extension = Path.GetExtension(file.FileName);
        var safeExtension = string.IsNullOrWhiteSpace(extension) ? string.Empty : extension.ToLowerInvariant();
        var objectKey = $"{entityType}/{entityId}/{Guid.NewGuid():N}{safeExtension}";

        try
        {
            if (file.Content.CanSeek)
            {
                file.Content.Position = 0;
            }

            await _minioClient.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(_settings.BucketName)
                    .WithObject(objectKey)
                    .WithStreamData(file.Content)
                    .WithObjectSize(file.Content.Length)
                    .WithContentType(file.ContentType),
                cancellationToken);

            return objectKey;

        }
        finally
        {
            await file.Content.DisposeAsync();
        }
    }

    public async Task<string?> GetReadUrlAsync(string? objectKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            return null;
        }
        var cacheKey = GetCacheKey(objectKey);
        var cached = await _cache.GetStringAsync(cacheKey, token: cancellationToken);
        if (!string.IsNullOrEmpty(cached))
        {
            return cached;
        }
        var url = await GetUrlAsync(objectKey);
        if (string.IsNullOrEmpty(url))
        {
            return null;
        }

        await _cache.SetStringAsync(
            cacheKey,
            url,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(Math.Max(1, _settings.PresignedUrlExpiryMinutes - 2))
            }, token: cancellationToken);

        return url;
    }

    public async Task DeleteAsync(string? objectKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            return;
        }
        
        await _cache.RemoveAsync(GetCacheKey(objectKey), cancellationToken);
        
        await EnsureBucketExistsAsync(cancellationToken);

        try
        {
            await _minioClient.RemoveObjectAsync(
                new RemoveObjectArgs()
                    .WithBucket(_settings.BucketName)
                    .WithObject(objectKey),
                cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Failed to delete MinIO object {ObjectKey}", objectKey);
        }
    }

    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        if (_bucketInitialized)
        {
            return;
        }

        await _bucketInitializationLock.WaitAsync(cancellationToken);
        try
        {
            if (_bucketInitialized)
            {
                return;
            }

            var bucketExists = await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(_settings.BucketName),
                cancellationToken);

            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(
                    new MakeBucketArgs().WithBucket(_settings.BucketName),
                    cancellationToken);
            }

            _bucketInitialized = true;
        }
        finally
        {
            _bucketInitializationLock.Release();
        }
    }
    
    private async Task<string?> GetUrlAsync(string? objectKey)
    {
        if (string.IsNullOrWhiteSpace(objectKey))
        {
            return null;
        }
        
        return await _minioClient.PresignedGetObjectAsync(
            new PresignedGetObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(objectKey)
                .WithExpiry(_settings.PresignedUrlExpiryMinutes * 60));
    }

    private string GetCacheKey(string? objectKey) => $"media_url_{objectKey?.GetHashCode()}";
}
