import axios from 'axios';
import type { PaginationRequest, PaginatedResponse } from '../types/pagination';
import type { Album } from '../pages/Albums';
import {ApiClient} from "../contexts/ApiClient.ts";

const API_URL = 'http://localhost:5071/albums';

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

export interface AlbumResponse extends Album {
  cursor: string;
}

interface AlbumSearchParams {
  cursor?: string;
  pageSize: number;
  titlePart?: string;
}

export const getAllAlbums = async (
    request: PaginationRequest<Date> & { searchTerm?: string } = { pageSize: 10 }) => {
  const params: AlbumSearchParams = {
    cursor: request.cursor?.toISOString(),
    pageSize: request.pageSize
  };

  if (request.searchTerm) {
    params.titlePart = request.searchTerm;
  }

  const endpoint = request.searchTerm ? `${API_URL}/by_name` : API_URL;

  const response = await axios.get<PaginatedResponse<Date, Album>>(endpoint, { params });
  return response.data;
};

export const getAlbumById = async (id: string) => {
  const response = await axios.get(`${API_URL}/${id}`);
  return response.data;
};

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