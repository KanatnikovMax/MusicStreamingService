import axios from 'axios';

const API_URL = 'http://localhost:5071';

export const ApiClient = axios.create({
    baseURL: API_URL
});

ApiClient.interceptors.request.use(config => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});