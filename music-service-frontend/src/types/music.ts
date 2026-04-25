export interface Artist {
  id: string;
  name: string;
  photoUrl?: string;
  albums?: AlbumSimple[];
  songs?: SongSimple[];
  createdAt: string;
}

export interface Album {
  id: string;
  title: string;
  releaseDate: string;
  photoUrl?: string;
  artists: ArtistSimple[];
  songs?: SongSimple[];
}

export interface Song {
  id: string;
  title: string;
  duration: number;
  trackNumber: number;
  albumId: string;
  artists: ArtistSimple[];
}

export interface Playlist {
  id: string;
  name: string;
  photoUrl?: string;
  userId: string;
  createdAt: string;
  updatedAt: string;
}

export interface ArtistSimple {
  id: string;
  name: string;
}

export interface AlbumSimple {
  id: string;
  title: string;
  releaseDate: string;
  photoUrl?: string;
  artists: ArtistSimple[];
}

export interface SongSimple {
  id: string;
  title: string;
  artists: ArtistSimple[];
  trackNumber: number;
  duration: number;
}
