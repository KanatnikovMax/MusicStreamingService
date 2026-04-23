import axios, { AxiosError } from 'axios';
import type { InternalAxiosRequestConfig } from 'axios';
import { refreshTokens } from '../services/authService';
import { clearAuthStorage, getAccessToken, getRefreshToken, setTokens } from '../services/authStorage';

const API_URL = 'http://localhost:5071';

interface RetryableRequestConfig extends InternalAxiosRequestConfig {
  _retry?: boolean;
}

export const ApiClient = axios.create({
  baseURL: API_URL
});

let refreshRequest: Promise<string> | null = null;

ApiClient.interceptors.request.use((config) => {
  const token = getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

ApiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as RetryableRequestConfig | undefined;

    if (!originalRequest || error.response?.status !== 401 || originalRequest._retry) {
      return Promise.reject(error);
    }

    const refreshToken = getRefreshToken();
    if (!refreshToken) {
      clearAuthStorage();
      return Promise.reject(error);
    }

    originalRequest._retry = true;

    try {
      refreshRequest ??= refreshTokens(refreshToken)
        .then((tokens) => {
          setTokens(tokens);
          return tokens.accessToken;
        })
        .finally(() => {
          refreshRequest = null;
        });

      const newAccessToken = await refreshRequest;
      originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;

      return ApiClient(originalRequest);
    } catch (refreshError) {
      clearAuthStorage();
      return Promise.reject(refreshError);
    }
  }
);
