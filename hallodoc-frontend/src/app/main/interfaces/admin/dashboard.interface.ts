import { RequestType } from '@main/enums';

export interface DashboardState {
  id: number;
  name: string;
  count: number;
  color: string;
  icon: string;
}

export interface AdminRequestData {
  id: number;
  patientFullName: string;
  dateOfBirth: string;
  requestorName: string;
  physicianName?: string;
  dateOfService?: string;
  phone: string;
  address: string;
  requestStatus?: string;
  requestType: RequestType;
  requestedDate: string;
}

export interface DashboardFilters {
  searchTerm: string;
  requestType: RequestType | null;
  page: number;
  pageSize: number;
}

export interface DashboardResponse {
  requests: AdminRequestData[];
  totalCount: number;
  stateCounts: DashboardState[];
}

export interface RequestAction {
  id: string;
  label: string;
  icon: string;
  action: string;
  disabled?: boolean;
} 