import { environment } from '../../../environments/environment';

const BASE_URL = environment.baseUrl;

// Auth Endpoints
export const AUTH_ENDPOINTS = {
  LOGIN: `${BASE_URL}/api/auth/login`,
  REGISTER: `${BASE_URL}/api/auth/register`,
  FORGOT_PASSWORD: `${BASE_URL}/api/auth/forgot-password`,
  RESET_PASSWORD: `${BASE_URL}/api/auth/reset-password`,
  GET_PROFILE: `${BASE_URL}/api/auth/profile`,
  UPDATE_PROFILE: `${BASE_URL}/api/auth/update-profile`,
  LOGOUT: `${BASE_URL}/api/auth/logout`
};

// Patient Endpoints
export const PATIENT_ENDPOINTS = {
  GET_REQUESTS: `${BASE_URL}/api/patient/requests`,
  GET_REQUEST_DETAILS: (requestId: number) => `${BASE_URL}/api/patient/requests/${requestId}`,
  GET_DOCUMENTS: (requestId: number) => `${BASE_URL}/api/patient/requests/${requestId}/documents`,
  DOWNLOAD_DOCUMENT: (requestId: number, documentId: number) => `${BASE_URL}/api/patient/requests/${requestId}/documents/${documentId}/download`
};

// Admin Dashboard Endpoints
export const ADMIN_DASHBOARD_ENDPOINTS = {
  GET_REQUESTS_BY_STATE: `${BASE_URL}/api/admin/dashboard/requests`,
  GET_STATE_COUNTS: `${BASE_URL}/api/admin/dashboard/state-counts`,
  EXPORT_REQUESTS: `${BASE_URL}/api/admin/dashboard/export`,
  EXPORT_ALL_REQUESTS: `${BASE_URL}/api/admin/dashboard/export-all`
};

// Request Forms Endpoints
export const REQUEST_FORM_ENDPOINTS = {
  CREATE_REQUEST: `${BASE_URL}/api/request`
};

// Common Endpoints
export const COMMON_ENDPOINTS = {
  GET_REGIONS: `${BASE_URL}/api/common/regions`,
  PING: `${BASE_URL}/api/ping`
}; 