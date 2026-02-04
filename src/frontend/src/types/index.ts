export enum AssetStatus {
  InStock = 0,
  Assigned = 1,
  Maintenance = 2,
  Retired = 3,
}

export interface Asset {
  id: string;
  assetCode: string;
  name: string;
  categoryId: string;
  status: AssetStatus;
  purchaseDate: string;
  location: string;
  assignedToUserId: string | null;
}

export interface AssetCategory {
  id: string;
  name: string;
}

export interface CreateAssetRequest {
  assetCode: string;
  name: string;
  categoryId: string;
  purchaseDate: string;
  location: string;
}

export interface AssignAssetRequest {
  assetId: string;
  userId: string;
}

export interface AuditLog {
  id: string;
  entityName: string;
  entityId: string;
  action: string;
  performedByUserId: string;
  timestamp: string;
  oldValue: string;
  newValue: string;
}

export interface User {
  id: string;
  email: string;
  role: 'Admin' | 'ITStaff' | 'Employee';
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
}

export interface PaginatedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}
