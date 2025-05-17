import axios from 'axios';

const API_URL = 'http://localhost:5071/authorization';

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
}

export const login = async (userName: string, password: string): Promise<TokenResponse> => {
  try {
    const formData = new FormData();
    formData.append('userName', userName);
    formData.append('password', password);

    const response = await axios.post<TokenResponse>(
      `${API_URL}/login`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );

    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      throw new Error(error.response?.data?.title || 'Login failed');
    }
    throw new Error('Unknown error occurred');
  }
};

export const register = async (formData: FormData): Promise<TokenResponse> => {
  try {
    const response = await axios.post<TokenResponse>(
      `${API_URL}/register`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );

    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      throw new Error(error.response?.data?.title || 'Registration failed');
    }
    throw new Error('Unknown error occurred');
  }
};