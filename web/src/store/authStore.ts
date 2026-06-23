import { create } from 'zustand';
import type { UserRole } from '../types';

interface AuthState {
  accessToken: string | null;
  role: UserRole | null;
  userId: string | null;
  initializing: boolean;
  setAuth: (token: string, role: UserRole, userId: string) => void;
  setToken: (token: string) => void;
  setInitializing: (v: boolean) => void;
  logout: () => void;
  isAuthenticated: () => boolean;
}

export const useAuthStore = create<AuthState>((set, get) => ({
  accessToken: null,
  role: null,
  userId: null,
  initializing: true,
  setAuth: (token, role, userId) => set({ accessToken: token, role, userId }),
  setToken: (token) => set({ accessToken: token }),
  setInitializing: (v) => set({ initializing: v }),
  logout: () => set({ accessToken: null, role: null, userId: null }),
  isAuthenticated: () => get().accessToken !== null,
}));
