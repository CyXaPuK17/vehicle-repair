import api from './client';
import type { ApiResponse, VehicleDto, RepairDto } from '../types';

export const getVehicles = () => api.get<ApiResponse<VehicleDto[]>>('/vehicles');
export const createVehicle = (data: Omit<VehicleDto, 'id' | 'isActive' | 'customerName'>) =>
  api.post<ApiResponse<string>>('/vehicles', data);
export const updateVehicle = (id: string, data: Omit<VehicleDto, 'id' | 'customerId' | 'customerName'>) =>
  api.put<ApiResponse<string>>(`/vehicles/${id}`, data);
export const getVehicleHistory = (id: string) =>
  api.get<ApiResponse<RepairDto[]>>(`/vehicles/${id}/repairs`);
