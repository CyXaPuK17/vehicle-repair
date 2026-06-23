import api from './client';
import type { ApiResponse, ExecutorDto } from '../types';

export interface ExecutorStatsDto {
  activeNow: number;
  doneThisMonth: number;
  doneThisYear: number;
  revenueThisMonth: number;
  revenueThisYear: number;
}

export const getExecutors = () => api.get<ApiResponse<ExecutorDto[]>>('/executors');
export const createExecutor = (data: Omit<ExecutorDto, 'id' | 'isActive' | 'createdAt'>) =>
  api.post<ApiResponse<string>>('/executors', data);
export const updateExecutor = (id: string, data: Partial<ExecutorDto>) =>
  api.put<ApiResponse<string>>(`/executors/${id}`, data);
export const getMyStats = () =>
  api.get<ApiResponse<ExecutorStatsDto>>('/executors/me/stats');
