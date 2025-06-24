import axios from 'axios';
import { ApiClient } from '../contexts/ApiClient';
import type { PaginationRequest, PaginatedResponse } from '../types/pagination';
import type { Artist, Album, Song } from '../types/music';

const API_URL = 'http://localhost:5071/artists';

export interface ArtistResponse {
  artists: Artist[];
}

interface ArtistSearchParams {
  cursor?: string;
  pageSize: number;
  namePart?: string;
}

export interface CreateArtistRequest {
  name: string;
  photo?: File;
}

export interface UpdateArtistRequest {
  name?: string;
  photo?: File;
}

export const getAllArtists = async (
    request: PaginationRequest<Date> & { searchTerm?: string } = { pageSize: 10 }
) => {
  const params: ArtistSearchParams = {
    cursor: request.cursor?.toISOString(),
    pageSize: request.pageSize
  };

  if (request.searchTerm) {
    params.namePart = request.searchTerm;
  }

  const endpoint = request.searchTerm ? `${API_URL}/by_name` : API_URL;
  const response = await axios.get<PaginatedResponse<string, Artist>>(endpoint, { params });

  return {
    items: response.data.items,
    cursor: response.data.cursor ? new Date(response.data.cursor) : undefined
  };
};

export const getArtistAlbums = async (
    artistId: string,
    request: PaginationRequest<Date> = { pageSize: 100 }
) => {
  const params = {
    cursor: request.cursor?.toISOString(),
    pageSize: request.pageSize
  };

  const response = await axios.get<PaginatedResponse<string, Album>>(
      `${API_URL}/${artistId}/albums`,
      { params }
  );

  return {
    items: response.data.items,
    cursor: response.data.cursor ? new Date(response.data.cursor) : undefined
  };
};

export const getArtistSongs = async (
    artistId: string,
    request: PaginationRequest<Date> = { pageSize: 100 }
) => {
  const params = {
    cursor: request.cursor?.toISOString(),
    pageSize: request.pageSize
  };

  const response = await axios.get<PaginatedResponse<string, Song>>(
      `${API_URL}/${artistId}/songs`,
      { params }
  );

  return {
    items: response.data.items,
    cursor: response.data.cursor ? new Date(response.data.cursor) : undefined
  };
};

export const getArtistSongsByTitle = async (
    artistId: string,
    titlePart: string,
    request: PaginationRequest<Date> = { pageSize: 100 }
) => {
  const params = {
    cursor: request.cursor?.toISOString(),
    pageSize: request.pageSize,
    titlePart
  };

  const response = await axios.get<PaginatedResponse<string, Song>>(
      `${API_URL}/${artistId}/songs/by_title`,
      { params }
  );

  return {
    items: response.data.items,
    cursor: response.data.cursor ? new Date(response.data.cursor) : undefined
  };
};

export const getArtistById = async (id: string): Promise<ArtistResponse> => {
  const response = await axios.get(`${API_URL}/${id}`);
  return response.data;
};

export const createArtist = async (data: CreateArtistRequest) => {
  const formData = new FormData();
  formData.append('name', data.name);
  if (data.photo) {
    formData.append('photo', data.photo);
  }

  const response = await ApiClient.post(`${API_URL}/create`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  });
  return response.data;
};

export const updateArtist = async (id: string, data: UpdateArtistRequest) => {
  const formData = new FormData();
  if (data.name) formData.append('name', data.name);
  if (data.photo) formData.append('photo', data.photo);

  const response = await ApiClient.put(`${API_URL}/update/${id}`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  });
  return response.data;
};

export const deleteArtist = async (id: string) => {
  await ApiClient.delete(`${API_URL}/delete/${id}`);
  return true;
};