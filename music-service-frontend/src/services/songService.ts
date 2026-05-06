import axios from 'axios';
import {ApiClient} from '../contexts/ApiClient';
import type {PaginatedResponse, PopularityPaginationRequest, PopularityCursor} from '../types/pagination';
import type {Song} from '../types/music';

const API_URL = 'http://localhost:5071/songs';

interface SongQueryParams {
  pageSize: number;
  cursorPlayCount?: number;
  cursorCreatedAt?: string;
  titlePart?: string;
}

export const getAllSongs = async (
    request: (PopularityPaginationRequest & { searchTerm?: string }) = { pageSize: 10 }
) => {
  const params: SongQueryParams = {
    pageSize: request.pageSize
  };

  if (request.cursorPlayCount !== undefined && request.cursorCreatedAt !== undefined) {
    params.cursorPlayCount = request.cursorPlayCount;
    params.cursorCreatedAt = request.cursorCreatedAt;
  }

  if (request.searchTerm?.trim()) {
    params.titlePart = request.searchTerm;
  }

  const response =
      await axios.get<PaginatedResponse<PopularityCursor, Song>>(API_URL, { params });

  return {
    items: response.data.items,
    cursor: response.data.cursor
  };
};

export const getSongById = async (id: string): Promise<Song> => {
  const response = await axios.get(`${API_URL}/${id}`);
  return response.data;
};

export const getSongAudioUrl = async (id: string): Promise<string> => {
  const response = await axios.get<{ audioUrl: string }>(`${API_URL}/${id}/url`);
  return response.data.audioUrl;
};

export const notifySongPlayed = async (userId: string, songId: string) => {
  await ApiClient.post(`/users/${userId}/songs/${songId}/played`);
};

// Admin functions
export const createSong = async (formData: FormData) => {
  const response = await ApiClient.post(`${API_URL}`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  });
  return response.data;
};

export const updateSong = async (id: string, formData: FormData) => {
  const response = await ApiClient.put(`${API_URL}/${id}`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  });
  return response.data;
};

export const deleteSong = async (id: string) => {
  await ApiClient.delete(`${API_URL}/${id}`);
  return true;
};