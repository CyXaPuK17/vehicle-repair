import api from './client';
import type { ApiResponse, UserDto, UserRole } from '../types';

export const getUsers = () => api.get<ApiResponse<UserDto[]>>('/users');
export const createUser = (data: {
  login: string;
  password: string;
  role: UserRole;
  customerId?: string;
  executorId?: string;
}) => api.post<ApiResponse<string>>('/users', data);
export const changePassword = (currentPassword: string, newPassword: string) =>
  api.patch<ApiResponse<string>>('/users/me/password', { currentPassword, newPassword });
export const updateUser = (id: string, data: {
  login: string;
  role: UserRole;
  customerId?: string;
  executorId?: string;
}) => api.put<ApiResponse<string>>(`/users/${id}`, data);
export const setUserActive = (id: string, isActive: boolean) =>
  api.patch<ApiResponse<string>>(`/users/${id}/active`, { isActive });
