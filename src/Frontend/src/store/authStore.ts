import { create } from 'zustand';
import { authApi } from '../services/api';
import type { UserDto } from '../types';

interface AuthState {
  user: UserDto | null;
  setAuth: (data: { accessToken: string; refreshToken: string; user: UserDto }) => void;
  logout: () => void;
  initialize: () => Promise<void>;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  setAuth: (data) => {
    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
    set({ user: data.user });
  },
  logout: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    set({ user: null });
  },
  initialize: async () => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      try {
        const user = await authApi.getCurrentUser();
        set({ user });
      } catch {
        set({ user: null });
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
      }
    }
  },
}));