import api from './client';
import type {
  ApiResponse, ReportByCustomerDto, ReportByExecutorDto,
  ReportByRepairsDto, ReportByVehicleDto
} from '../types';

export const reportByCustomer = (from: string, to: string, customerId?: string) =>
  api.get<ApiResponse<ReportByCustomerDto>>('/reports/by-customer', { params: { from, to, customerId } });

export const reportByExecutor = (from: string, to: string, executorId?: string) =>
  api.get<ApiResponse<ReportByExecutorDto>>('/reports/by-executor', { params: { from, to, executorId } });

export const reportByRepairs = (from: string, to: string) =>
  api.get<ApiResponse<ReportByRepairsDto>>('/reports/by-repairs', { params: { from, to } });

export const reportByVehicle = (from: string, to: string, vehicleId?: string) =>
  api.get<ApiResponse<ReportByVehicleDto>>('/reports/by-vehicle', { params: { from, to, vehicleId } });

export const exportReport = (
  type: 'by-customer' | 'by-executor' | 'by-repairs' | 'by-vehicle',
  format: 'xlsx' | 'pdf',
  from: string,
  to: string,
  entityId?: string
) => {
  const params: Record<string, string> = { from, to, export: format };
  if (entityId) {
    if (type === 'by-customer') params.customerId = entityId;
    else if (type === 'by-executor') params.executorId = entityId;
    else if (type === 'by-vehicle') params.vehicleId = entityId;
  }
  return api.get(`/reports/${type}`, { params, responseType: 'blob' });
};
