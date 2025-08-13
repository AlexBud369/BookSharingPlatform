import axios from 'axios';
import type { UserDto } from '../types';
import { useLoadingStore } from '../store/loadingStore';
import { useAuthStore } from '../store/authStore';

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
});

api.interceptors.request.use((config) => {
  useLoadingStore.getState().setLoading(true);
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => {
    useLoadingStore.getState().setLoading(false);
    return response;
  },
  async (error) => {
    useLoadingStore.getState().setLoading(false);
    if (error.response?.status === 401) {
      try {
        const refreshToken = localStorage.getItem('refreshToken');
        if (refreshToken) {
          const { data } = await authApi.refreshToken({ refreshToken });
          localStorage.setItem('accessToken', data.accessToken);
          localStorage.setItem('refreshToken', data.refreshToken);
          useAuthStore.getState().setAuth(data);
          error.config.headers.Authorization = `Bearer ${data.accessToken}`;
          return api.request(error.config);
        }
      } catch {
        useAuthStore.getState().logout();
      }
    }
    throw error;
  },
);

export const authApi = {
  login: (data: { email: string; password: string }) => api.post('/auth/login', data).then((res) => res.data),
  register: (data: { username: string; email: string; password: string }) =>
    api.post('/auth/register', data).then((res) => res.data),
  logout: () => api.post('/auth/logout'),
  refreshToken: (data: { refreshToken: string }) => api.post('/auth/refresh-token', data).then((res) => res.data),
  getCurrentUser: () => api.get('/users/me').then((res) => res.data),
};

export const userApi = {
  getUsers: (params: { pageNumber: number; pageSize: number }) =>
    api.get('/users', { params }).then((res) => res.data),
  blockUser: (userId: string, adminId: string) => api.put(`/users/${userId}/block`, { adminId }),
  updateUser: (userId: string, data: { username?: string; email?: string; password?: string }) =>
    api.put(`/users/${userId}`, data).then((res) => res.data),
  getCurrentUser: async (): Promise<UserDto> => {
    const response = await axios.get('/api/users/me');
    return response.data;
  },
};

export const bookApi = {
  getBooks: (params: { pageNumber: number; pageSize: number; sortDescending?: boolean; searchQuery?: string; tagIds?: string[] }) =>
    api.get('/books', { params }).then((res) => res.data),
  getBookById: (id: string) => api.get(`/books/${id}`).then((res) => res.data),
  createBook: (data: { title: string; author: string; description?: string; tagIds: string[] }) =>
    api.post('/books', data).then((res) => res.data),
  updateBook: (id: string, data: { title?: string; author?: string; description?: string; tagIds?: string[] }) =>
    api.put(`/books/${id}`, data).then((res) => res.data),
  deleteBook: (id: string) => api.delete(`/books/${id}`),
  uploadCover: (id: string, file: File) => {
    const formData = new FormData();
    formData.append('coverImage', file);
    return api.post(`/books/${id}/cover`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },
};

export const tagApi = {
  getTags: (params: { pageNumber: number; pageSize: number }) =>
    api.get('/tags', { params }).then((res) => res.data),
  createTag: (data: { tagName: string }) =>
    api.post('/tags', data).then((res) => res.data),
};