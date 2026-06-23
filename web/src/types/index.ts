export type UserRole = 'ManagementCompany' | 'Customer' | 'Executor';
export type RepairStatus = 'Received' | 'InProgress' | 'Completed' | 'Issued';

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
}

export interface TokenResponse {
  accessToken: string;
  role: UserRole;
  userId: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T | null;
  error: { code: string; message: string } | null;
}

export interface CustomerDto {
  id: string;
  inn: string;
  name: string;
  contactPerson?: string;
  phone?: string;
  email?: string;
  isActive: boolean;
  createdAt: string;
}

export interface ExecutorDto {
  id: string;
  inn: string;
  name: string;
  address?: string;
  phone?: string;
  email?: string;
  isActive: boolean;
  createdAt: string;
}

export interface VehicleDto {
  id: string;
  licensePlate: string;
  make: string;
  model: string;
  year?: number;
  vin?: string;
  vehicleType: number;
  isActive: boolean;
  customerId: string;
  customerName: string;
}

export interface RepairTypeDto {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
}

export interface RepairDto {
  id: string;
  vehicleId: string;
  licensePlate: string;
  vehicleMakeModel: string;
  customerId: string;
  customerName: string;
  executorId: string;
  executorName: string;
  repairTypeId: string;
  repairTypeName: string;
  receivedAt: string;
  issuedAt?: string;
  cost: number;
  mileage: number;
  status: RepairStatus;
  comment?: string;
  createdAt: string;
}

export interface UserDto {
  id: string;
  login: string;
  role: UserRole;
  customerId?: string;
  executorId?: string;
  linkedEntityName?: string;
  isActive: boolean;
  lastLoginAt?: string;
  createdAt: string;
}

export interface ReportByCustomerDto {
  rows: ReportByCustomerRow[];
  totalCount: number;
  totalCost: number;
}
export interface ReportByCustomerRow {
  customerId: string;
  customerName: string;
  customerINN: string;
  repairCount: number;
  totalCost: number;
}

export interface ReportByExecutorDto {
  rows: ReportByExecutorRow[];
  totalCount: number;
  totalCost: number;
}
export interface ReportByExecutorRow {
  executorId: string;
  executorName: string;
  executorINN: string;
  repairCount: number;
  totalCost: number;
}

export interface ReportByRepairsDto {
  rows: ReportByRepairsRow[];
  totalCount: number;
  totalCost: number;
}
export interface ReportByRepairsRow {
  repairId: string;
  licensePlate: string;
  vehicleMakeModel: string;
  customerName: string;
  executorName: string;
  repairTypeName: string;
  receivedAt: string;
  issuedAt?: string;
  mileage: number;
  cost: number;
}

export interface ReportByVehicleDto {
  rows: ReportByVehicleRow[];
}
export interface ReportByVehicleRow {
  vehicleId: string;
  licensePlate: string;
  makeModel: string;
  customerName: string;
  totalCost: number;
  mileageDelta: number;
  repairs: ReportByVehicleRepairRow[];
}
export interface ReportByVehicleRepairRow {
  repairTypeName: string;
  executorName: string;
  receivedAt: string;
  mileage: number;
  cost: number;
}
