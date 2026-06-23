import api from './client';
import type { ApiResponse, RepairTypeDto } from '../types';

export const getRepairTypes = () => api.get<ApiResponse<RepairTypeDto[]>>('/repair-types');
export const createRepairType = (data: { name: string; description?: string }) =>
  api.post<ApiResponse<string>>('/repair-types', data);
export const updateRepairType = (id: string, data: { name: string; description?: string; isActive: boolean }) =>
  api.put<ApiResponse<string>>(`/repair-types/${id}`, data);
