import { ApiClient } from '../contexts/ApiClient';
import type { PaginationRequest, PaginatedResponse } from '../types/pagination';
import type { Album, Song } from '../types/music';

const API_URL = 'http://localhost:5071/users';

export interface UserAlbumsResponse {
  items: Album[];
  cursor?: Date;
}

export interface UserSongsResponse {
  items: Song[];
  cursor?: Date;
}

// User actions
export const addSongToAccount = async (userId: string, songId: string) => {
  const response = await ApiClient.post(`${API_URL}/${userId}/add_song`, null, {
    params: { songId }
  });
  return response.data;
};

export const addAlbumToAccount = async (userId: string, albumId: string) => {
  const response = await ApiClient.post(`${API_URL}/${userId}/add_album`, null, {
    params: { albumId }
  });
  return response.data;
};

export const deleteSongFromAccount = async (userId: string, songId: string) => {
  const response = await ApiClient.delete(`${API_URL}/${userId}/delete_song`, {
    params: { songId }
  });
  return response.data;
};

export const deleteAlbumFromAccount = async (userId: string, albumId: string) => {
  const response = await ApiClient.delete(`${API_URL}/${userId}/delete_album`, {
    params: { albumId }
  });
  return response.data;
};

export const getUserAlbums = async (
    userId: string,
    request: PaginationRequest<Date> = { pageSize: 100 }
): Promise<UserAlbumsResponse> => {
  const params = {
    cursor: request.cursor?.toISOString(),
    pageSize: request.pageSize
  };

  const response = await ApiClient.get<PaginatedResponse<string, Album>>(
      `${API_URL}/${userId}/albums`,
      { params }
  );

  return {
    items: response.data.items,
    cursor: response.data.cursor ? new Date(response.data.cursor) : undefined
  };
};

export const getUserSongs = async (
    userId: string,
    request: PaginationRequest<Date> = { pageSize: 100 }
): Promise<UserSongsResponse> => {
  const params = {
    cursor: request.cursor?.toISOString(),
    pageSize: request.pageSize
  };

  const response = await ApiClient.get<PaginatedResponse<string, Song>>(
      `${API_URL}/${userId}/songs`,
      { params }
  );

  return {
    items: response.data.items,
    cursor: response.data.cursor ? new Date(response.data.cursor) : undefined
  };
};

export const getUserAlbumsByTitle = async (
  userId: string,
  titlePart: string,
  request: PaginationRequest<Date> = { pageSize: 10 }
) => {
  const params = {
    cursor: request.cursor?.toISOString(),
    pageSize: request.pageSize,
    titlePart
  };

  const response = await ApiClient.get<PaginatedResponse<Date, Album>>(
    `${API_URL}/${userId}/albums/by_title`,
    { params }
  );
  return response.data;
};

export const getUserSongsByName = async (
  userId: string,
  namePart: string,
  request: PaginationRequest<Date> = { pageSize: 10 }
) => {
  const params = {
    cursor: request.cursor?.toISOString(),
    pageSize: request.pageSize,
    namePart
  };

  const response = await ApiClient.get<PaginatedResponse<Date, Song>>(
    `${API_URL}/${userId}/songs/by_name`,
    { params }
  );
  return response.data;
};

// Account settings
interface ChangeEmailRequest {
  email: string;
  password: string;
}

interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

interface ChangeUsernameRequest {
  userName: string;
  password: string;
}

export const changeEmail = async (username: string, data: ChangeEmailRequest) => {
  const response = await ApiClient.post(`/account/${username}/change_email`, data);
  return response.data;
};

export const changePassword = async (username: string, data: ChangePasswordRequest) => {
  const response = await ApiClient.post(`/account/${username}/change_password`, data);
  return response.data;
};

export const changeUsername = async (username: string, data: ChangeUsernameRequest) => {
  const response = await ApiClient.post(`/account/${username}/change_username`, data);
  return response.data;
};
