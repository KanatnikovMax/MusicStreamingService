namespace MusicStreamingService.MediaLibrary.Settings;

public class MinioSettings
{
    public string Endpoint { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string BucketName { get; set; }
    public bool UseSsl { get; set; }
    public int PresignedUrlExpiryMinutes { get; set; }
}