import { ApiClient } from '../contexts/ApiClient';
import type { PaginatedResponse, PaginationRequest } from '../types/pagination';
import type { Playlist, Song } from '../types/music';

const API_URL = 'http://localhost:5071/users';

interface PlaylistSearchParams {
  cursor?: string;
  pageSize: number;
  namePart?: string;
}

interface PlaylistSongsParams {
  cursor?: number;
  pageSize: number;
  namePart?: string;
}

export interface PlaylistRequest {
  name: string;
}

export const getUserPlaylists = async (
    userId: string,
    request: PaginationRequest<Date> & { searchTerm?: string } = { pageSize: 100 }
) => {
  const params: PlaylistSearchParams = {
    cursor: request.cursor?.toISOString(),
    pageSize: request.pageSize
  };

  if (request.searchTerm?.trim()) {
    params.namePart = request.searchTerm;
  }

  const response = await ApiClient.get<PaginatedResponse<string, Playlist>>(
      `${API_URL}/${userId}/playlists`,
      { params }
  );

  return {
    items: response.data.items,
    cursor: response.data.cursor ? new Date(response.data.cursor) : undefined
  };
};

export const getUserPlaylistById = async (userId: string, playlistId: string) => {
  let cursor: Date | undefined;

  do {
    const response = await getUserPlaylists(userId, { cursor, pageSize: 100 });
    const playlist = response.items.find(item => item.id === playlistId);
    if (playlist) {
      return playlist;
    }

    cursor = response.cursor;
  } while (cursor);

  return null;
};

export const createPlaylist = async (userId: string, data: PlaylistRequest) => {
  const response = await ApiClient.post<Playlist>(`${API_URL}/${userId}/playlists`, data);
  return response.data;
};

export const updatePlaylist = async (userId: string, playlistId: string, data: PlaylistRequest) => {
  const response = await ApiClient.patch<Playlist>(
      `${API_URL}/${userId}/playlists/${playlistId}`,
      data
  );
  return response.data;
};

export const deletePlaylist = async (userId: string, playlistId: string) => {
  const response = await ApiClient.delete<Playlist>(`${API_URL}/${userId}/playlists/${playlistId}`);
  return response.data;
};

export const getPlaylistSongs = async (
    userId: string,
    playlistId: string,
    request: PaginationRequest<number> & { searchTerm?: string } = { pageSize: 100 }
) => {
  const params: PlaylistSongsParams = {
    cursor: request.cursor,
    pageSize: request.pageSize
  };

  if (request.searchTerm?.trim()) {
    params.namePart = request.searchTerm;
  }

  const response = await ApiClient.get<PaginatedResponse<number, Song>>(
      `${API_URL}/${userId}/playlists/${playlistId}/songs`,
      { params }
  );

  return response.data;
};

export const addSongToPlaylist = async (userId: string, playlistId: string, songId: string) => {
  const response = await ApiClient.post(
      `${API_URL}/${userId}/playlists/${playlistId}/songs/${songId}`
  );
  return response.data;
};

export const removeSongFromPlaylist = async (userId: string, playlistId: string, songId: string) => {
  const response = await ApiClient.delete(
      `${API_URL}/${userId}/playlists/${playlistId}/songs/${songId}`
  );
  return response.data;
};
