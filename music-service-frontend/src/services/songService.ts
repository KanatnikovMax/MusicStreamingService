import axios from 'axios';
import { ApiClient } from '../contexts/ApiClient';
import type { PaginationRequest, PaginatedResponse } from '../types/pagination';
import type { Song } from '../types/music';

const API_URL = 'http://localhost:5071/songs';

export interface SongResponse extends Song {
  songs: Song[];
}

interface SongSearchParams {
  cursor?: string;
  pageSize: number;
  titlePart?: string;
}

export const getAllSongs = async (
    request: PaginationRequest<Date> & { searchTerm?: string } = { pageSize: 10 }
) => {
  const params: SongSearchParams = {
    cursor: request.cursor?.toISOString(),
    pageSize: request.pageSize
  };

  if (request.searchTerm) {
    params.titlePart = request.searchTerm;
  }

  const endpoint = request.searchTerm ? `${API_URL}/by_title` : API_URL;
  const response = await axios.get<PaginatedResponse<string, Song>>(endpoint, { params });

  return {
    items: response.data.items,
    cursor: response.data.cursor ? new Date(response.data.cursor) : undefined
  };
};

export const getSongById = async (id: string): Promise<SongResponse> => {
  const response = await axios.get(`${API_URL}/${id}`);
  return response.data;
};

export const getSongAudio = async (id: string): Promise<Blob> => {
  const response = await axios.get(`${API_URL}/${id}/audio`, {
    responseType: 'blob'
  });
  return response.data;
};

// Admin functions
export const createSong = async (formData: FormData) => {
  const response = await ApiClient.post(`${API_URL}/create`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  });
  return response.data;
};

export const updateSong = async (id: string, formData: FormData) => {
  const response = await ApiClient.put(`${API_URL}/update/${id}`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  });
  return response.data;
};

export const deleteSong = async (id: string) => {
  await ApiClient.delete(`${API_URL}/delete/${id}`);
  return true;
};