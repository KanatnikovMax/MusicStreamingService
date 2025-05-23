﻿namespace MusicStreamingService.DataAccess.Postgres.Entities;

public class Song() : BaseEntity
{
    public string Title { get; set; } 
    // Duration in seconds
    public int Duration { get; set; }
    public int TrackNumber { get; set; }
    public Guid CassandraId { get; set; }
    
    public Guid AlbumId { get; set; }
    public Album Album { get; set; }
    
    public ICollection<User>? Users { get; set; }
    public ICollection<Artist> Artists { get; set; }
    
    public ICollection<UserSong>? UsersSongs { get; set; }
}