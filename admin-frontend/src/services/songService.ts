import axios from 'axios';
import type { PaginationRequest, PaginatedResponse } from '../types/pagination';
import type { Song } from '../pages/Songs';
import {ApiClient} from "../contexts/ApiClient.ts";

const API_URL = 'http://localhost:5071/songs';

export interface CreateSongRequest {
  title: string;
  duration: number;
  trackNumber: number;
  albumId: string;
  artists: string[];
  audioFile: File;
}

export interface UpdateSongRequest {
  title?: string;
  trackNumber?: number;
}

export interface SongResponse extends Song {
  cursor: string;
}

interface SongSearchParams {
  cursor?: string;
  pageSize: number;
  namePart?: string;
}

export const getAllSongs = async (
    request: PaginationRequest<Date> & { searchTerm?: string } = { pageSize: 10 }) => {
  const params: SongSearchParams = {
    cursor: request.cursor?.toISOString(),
    pageSize: request.pageSize
  };

  if (request.searchTerm) {
    params.namePart = request.searchTerm;
  }

  const endpoint = request.searchTerm ? `${API_URL}/by_title` : API_URL;

  const response = await axios.get<PaginatedResponse<Date, Song>>(endpoint, { params });
  return response.data;
};

export const getSongById = async (id: string) => {
  const response = await axios.get(`${API_URL}/${id}`);
  return response.data;
};

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