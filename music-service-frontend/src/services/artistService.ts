import axios from 'axios';
import {ApiClient} from '../contexts/ApiClient';
import type {PaginatedResponse, PaginationRequest, PopularityPaginationRequest, PopularityCursor} from '../types/pagination';
import type {Album, Artist, Song} from '../types/music';

const API_URL = 'http://localhost:5071/artists';

interface ArtistQueryParams {
  pageSize: number;
  cursorPlayCount?: number;
  cursorCreatedAt?: string;
  namePart?: string;
}

interface ArtistSongsQueryParams {
  pageSize: number;
  cursorPlayCount?: number;
  cursorCreatedAt?: string;
  titlePart?: string;
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
    request: (PopularityPaginationRequest & { searchTerm?: string }) = { pageSize: 10 }
) => {
  const params: ArtistQueryParams = {
    pageSize: request.pageSize
  };

  if (request.cursorPlayCount !== undefined && request.cursorCreatedAt !== undefined) {
    params.cursorPlayCount = request.cursorPlayCount;
    params.cursorCreatedAt = request.cursorCreatedAt;
  }

  if (request.searchTerm?.trim()) {
    params.namePart = request.searchTerm;
  }

  const response =
      await axios.get<PaginatedResponse<PopularityCursor, Artist>>(API_URL, { params });

  return {
    items: response.data.items,
    cursor: response.data.cursor
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
    request: PopularityPaginationRequest = { pageSize: 100 }
) => {
  const params: ArtistSongsQueryParams = {
    pageSize: request.pageSize
  };

  if (request.cursorPlayCount !== undefined && request.cursorCreatedAt !== undefined) {
    params.cursorPlayCount = request.cursorPlayCount;
    params.cursorCreatedAt = request.cursorCreatedAt;
  }

  const response = await axios.get<PaginatedResponse<PopularityCursor, Song>>(
      `${API_URL}/${artistId}/songs`,
      { params }
  );

  return {
    items: response.data.items,
    cursor: response.data.cursor
  };
};

export const getArtistSongsByTitle = async (
    artistId: string,
    titlePart: string,
    request: PopularityPaginationRequest = { pageSize: 100 }
) => {
  const params: ArtistSongsQueryParams = {
    pageSize: request.pageSize,
    titlePart
  };

  if (request.cursorPlayCount !== undefined && request.cursorCreatedAt !== undefined) {
    params.cursorPlayCount = request.cursorPlayCount;
    params.cursorCreatedAt = request.cursorCreatedAt;
  }

  const response = await axios.get<PaginatedResponse<PopularityCursor, Song>>(
      `${API_URL}/${artistId}/songs`,
      { params }
  );

  return {
    items: response.data.items,
    cursor: response.data.cursor
  };
};

export const getArtistById = async (id: string): Promise<Artist> => {
  const response = await axios.get(`${API_URL}/${id}`);
  return response.data;
};

export const createArtist = async (data: CreateArtistRequest) => {
  const formData = new FormData();
  formData.append('name', data.name);
  if (data.photo) {
    formData.append('photo', data.photo);
  }

  const response = await ApiClient.post(`${API_URL}`, formData, {
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

  const response = await ApiClient.put(`${API_URL}/${id}`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  });
  return response.data;
};

export const deleteArtist = async (id: string) => {
  await ApiClient.delete(`${API_URL}/${id}`);
  return true;
};