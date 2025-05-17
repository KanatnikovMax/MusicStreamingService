import axios from 'axios';
import { ApiClient } from '../contexts/ApiClient';
import type { PaginationRequest, PaginatedResponse } from '../types/pagination';
import type { Album, Song } from '../types/music';

const API_URL = 'http://localhost:5071/albums';

export interface AlbumResponse {
  albums: Album[];
}

interface AlbumSearchParams {
  cursor?: string;
  pageSize: number;
  titlePart?: string;
}

export interface CreateAlbumRequest {
  title: string;
  releaseDate: string;
  artists: string[];
}

export interface UpdateAlbumRequest {
  title?: string;
  releaseDate?: string;
  artists?: string[];
}

export const getAllAlbums = async (
    request: PaginationRequest<Date> & { searchTerm?: string } = { pageSize: 10 }
) => {
  const params: AlbumSearchParams = {
    cursor: request.cursor?.toISOString(),
    pageSize: request.pageSize
  };

  if (request.searchTerm) {
    params.titlePart = request.searchTerm;
  }

  const endpoint = request.searchTerm ? `${API_URL}/by_name` : API_URL;
  const response = await axios.get<PaginatedResponse<string, Album>>(endpoint, { params });

  return {
    items: response.data.items,
    cursor: response.data.cursor ? new Date(response.data.cursor) : undefined
  };
};

export const getAlbumById = async (id: string): Promise<AlbumResponse> => {
  const response = await axios.get(`${API_URL}/${id}`);
  return response.data;
};

export const getAlbumSongs = async (
    albumId: string,
    request: PaginationRequest<number> = { pageSize: 100 }
) => {
  const params = {
    cursor: request.cursor,
    pageSize: request.pageSize
  };

  const response = await axios.get<PaginatedResponse<number, Song>>(
      `${API_URL}/${albumId}/songs`,
      { params }
  );
  return response.data;
};

// Admin functions
export const createAlbum = async (data: CreateAlbumRequest) => {
  const response = await ApiClient.post(`${API_URL}/create`, data);
  return response.data;
};

export const updateAlbum = async (id: string, data: UpdateAlbumRequest) => {
  const response = await ApiClient.put(`${API_URL}/update/${id}`, data);
  return response.data;
};

export const deleteAlbum = async (id: string) => {
  await ApiClient.delete(`${API_URL}/delete/${id}`);
  return true;
};