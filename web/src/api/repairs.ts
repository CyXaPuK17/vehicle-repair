import api from './client';
import type { ApiResponse, PagedResult, RepairDto } from '../types';

export const getRepairs = (from?: string, to?: string, page = 1, pageSize = 50) =>
  api.get<ApiResponse<PagedResult<RepairDto>>>('/repairs', { params: { from, to, page, pageSize } });

export const createRepair = (data: {
  vehicleId: string;
  repairTypeId: string;
  receivedAt: string;
  cost: number;
  mileage: number;
  comment?: string;
}) => api.post<ApiResponse<string>>('/repairs', data);

export const updateRepair = (id: string, data: {
  repairTypeId: string;
  receivedAt: string;
  cost: number;
  mileage: number;
  comment?: string;
}) => api.put<ApiResponse<string>>(`/repairs/${id}`, data);

export const issueRepair = (id: string, issuedAt: string) =>
  api.patch<ApiResponse<string>>(`/repairs/${id}/issue`, { issuedAt });
