import api from './client';
import type { ApiResponse, CustomerDto } from '../types';

export const getCustomers = () => api.get<ApiResponse<CustomerDto[]>>('/customers');
export const getCustomer = (id: string) => api.get<ApiResponse<CustomerDto>>(`/customers/${id}`);
export const createCustomer = (data: Omit<CustomerDto, 'id' | 'isActive' | 'createdAt'>) =>
  api.post<ApiResponse<string>>('/customers', data);
export const updateCustomer = (id: string, data: Partial<CustomerDto>) =>
  api.put<ApiResponse<string>>(`/customers/${id}`, data);
