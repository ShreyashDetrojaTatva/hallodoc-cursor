import { environment } from '../../../environments/environment';

const BASE_URL = environment.baseUrl;

// Auth Endpoints
export const AUTH_ENDPOINTS = {
  LOGIN: `${BASE_URL}/api/auth/login`,
  FORGOT_PASSWORD: `${BASE_URL}/api/auth/forgot-password`,
  RESET_PASSWORD: `${BASE_URL}/api/auth/reset-password`,
  GET_PROFILE: `${BASE_URL}/api/auth/profile`,
  UPDATE_PROFILE: `${BASE_URL}/api/auth/profile`, // PUT request to same endpoint
  LOGOUT: `${BASE_URL}/api/auth/logout`
};

// Patient Endpoints (using RequestController)
export const PATIENT_ENDPOINTS = {
  GET_REQUESTS: `${BASE_URL}/api/request/patient`,
  GET_REQUEST_DETAILS: (requestId: number) => `${BASE_URL}/api/request/${requestId}`,
  GET_DOCUMENTS: (requestId: number) => `${BASE_URL}/api/request/${requestId}/documents`,
  DOWNLOAD_DOCUMENT: (documentId: number) => `${BASE_URL}/api/request/documents/${documentId}/download`
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