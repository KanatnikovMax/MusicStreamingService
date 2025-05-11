import axios from 'axios';

const API_URL = 'http://localhost:5071';

export const ApiClient = axios.create({
    baseURL: API_URL,
    headers: {
        Authorization: `Bearer ${localStorage.getItem('token')}`
    }
});