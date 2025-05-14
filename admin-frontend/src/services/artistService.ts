import axios from 'axios';
import type {PaginationRequest, PaginatedResponse} from '../types/pagination';
import type {Artist} from '../pages/Artists'
import {ApiClient} from "../contexts/ApiClient.ts";

const API_URL = 'http://localhost:5071/artists';

export interface CreateArtistRequest {
  name: string;
}

export interface UpdateArtistRequest {
  name?: string;
}

interface ArtistSearchParams {
  cursor?: string;
  pageSize: number;
  namePart?: string;
}

export const getAllArtists = async (
    request: PaginationRequest<Date> & { searchTerm?: string } = { pageSize: 10 }) => {
  const params: ArtistSearchParams = {
    cursor: request.cursor?.toISOString(),
    pageSize: request.pageSize
  };

  if (request.searchTerm) {
    params.namePart = request.searchTerm;
  }

  const endpoint = request.searchTerm ? `${API_URL}/by_name` : API_URL;

  const response = await axios.get<PaginatedResponse<Date, Artist>>(endpoint, { params });
  return response.data;
};

export const getArtistById = async (id: string) => {
  const response = await axios.get(`${API_URL}/${id}`);
  return response.data;
};

export const createArtist = async (data: CreateArtistRequest) => {
  const response = await ApiClient.post(`${API_URL}/create`, data);
  return response.data;
};

export const updateArtist = async (id: string, data: UpdateArtistRequest) => {
  const response = await ApiClient.put(`${API_URL}/update/${id}`, data);
  return response.data;
};

export const deleteArtist = async (id: string) => {
  await ApiClient.delete(`${API_URL}/delete/${id}`);
  return true;
};