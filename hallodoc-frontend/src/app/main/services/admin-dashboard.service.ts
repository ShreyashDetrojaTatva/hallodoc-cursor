import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DashboardResponse, DashboardFilters, AdminRequestData, DashboardState } from '@main/interfaces/admin/dashboard.interface';
import { ADMIN_DASHBOARD_ENDPOINTS } from '@main/constants';
import { DashboardRequestStatus } from '@main/enums';

@Injectable({
  providedIn: 'root'
})
export class AdminDashboardService {

  constructor(private http: HttpClient) {}

  getRequestsByState(state: DashboardRequestStatus, filters: DashboardFilters): Observable<DashboardResponse> {
    let params = new HttpParams()
      .set('state', state)
      .set('page', filters.page.toString())
      .set('pageSize', filters.pageSize.toString());

    if (filters.searchTerm) {
      params = params.set('searchTerm', filters.searchTerm);
    }

    if (filters.requestType !== null) {
      params = params.set('requestType', filters.requestType.toString());
    }

    return this.http.get<DashboardResponse>(ADMIN_DASHBOARD_ENDPOINTS.GET_REQUESTS_BY_STATE, { params });
  }

  getStateCounts(): Observable<DashboardState[]> {
    return this.http.get<DashboardState[]>(ADMIN_DASHBOARD_ENDPOINTS.GET_STATE_COUNTS);
  }

  // Stub methods for export functionality
  exportRequests(state: DashboardRequestStatus, filters: DashboardFilters): Observable<Blob> {
    let params = new HttpParams()
      .set('state', state)
      .set('page', filters.page.toString())
      .set('pageSize', filters.pageSize.toString());

    if (filters.searchTerm) {
      params = params.set('searchTerm', filters.searchTerm);
    }

    if (filters.requestType !== null) {
      params = params.set('requestType', filters.requestType.toString());
    }

    return this.http.get(ADMIN_DASHBOARD_ENDPOINTS.EXPORT_REQUESTS, { 
      params, 
      responseType: 'blob' 
    });
  }

  exportAllRequests(filters: DashboardFilters): Observable<Blob> {
    let params = new HttpParams()
      .set('page', filters.page.toString())
      .set('pageSize', filters.pageSize.toString());

    if (filters.searchTerm) {
      params = params.set('searchTerm', filters.searchTerm);
    }

    if (filters.requestType !== null) {
      params = params.set('requestType', filters.requestType.toString());
    }

    return this.http.get(ADMIN_DASHBOARD_ENDPOINTS.EXPORT_ALL_REQUESTS, { 
      params, 
      responseType: 'blob' 
    });
  }
} 