import api from './client';
import type { ApiResponse, TokenResponse } from '../types';

export const login = (login: string, password: string) =>
  api.post<ApiResponse<TokenResponse>>('/auth/login', { login, password });

export const logout = () => api.post('/auth/logout');
