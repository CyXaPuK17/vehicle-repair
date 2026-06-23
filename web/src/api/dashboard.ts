import api from './client';
import type { ApiResponse } from '../types';

export interface ExecutorStatDto { name: string; count: number; totalCost: number; }
export interface DashboardDto {
  received: number;
  inProgress: number;
  completed: number;
  repairsForPeriod: number;
  revenueForPeriod: number;
  topExecutors: ExecutorStatDto[];
}

export const getDashboard = (from: string, to: string, topCount: number) =>
  api.get<ApiResponse<DashboardDto>>('/dashboard', { params: { from, to, topCount } });
