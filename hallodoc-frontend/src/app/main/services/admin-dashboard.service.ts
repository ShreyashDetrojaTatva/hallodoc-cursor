import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminRequestData, DashboardState } from '@main/interfaces/admin/dashboard.interface';
import { PaginationRequest, PaginationResponse, PaginationDashboardFilters } from '@main/interfaces/pagination';
import { ADMIN_DASHBOARD_ENDPOINTS } from '@main/constants';
import { DashboardRequestStatus } from '@main/enums';

@Injectable({
  providedIn: 'root'
})
export class AdminDashboardService {

  constructor(private http: HttpClient) {}

  getRequestsByState(state: DashboardRequestStatus, request: PaginationRequest<PaginationDashboardFilters>): Observable<PaginationResponse<AdminRequestData>> {
    let params = new HttpParams()
      .set('state', state)
      .set('pageIndex', request.pageIndex.toString())
      .set('pageSize', request.pageSize.toString());

    if (request.searchString) {
      params = params.set('searchString', request.searchString);
    }

    if (request.sortColumn) {
      params = params.set('sortColumn', request.sortColumn);
    }

    if (request.sortDirection) {
      params = params.set('sortDirection', request.sortDirection);
    }

    if (request.filters.requestType !== undefined && request.filters.requestType !== null) {
      params = params.set('requestType', request.filters.requestType.toString());
    }

    if (request.filters.regionId !== undefined && request.filters.regionId !== null) {
      params = params.set('regionId', request.filters.regionId.toString());
    }

    if (request.filters.fromDate) {
      params = params.set('fromDate', request.filters.fromDate);
    }

    if (request.filters.toDate) {
      params = params.set('toDate', request.filters.toDate);
    }

    return this.http.get<PaginationResponse<AdminRequestData>>(ADMIN_DASHBOARD_ENDPOINTS.GET_REQUESTS_BY_STATE, { params });
  }

  getStateCounts(): Observable<DashboardState[]> {
    return this.http.get<DashboardState[]>(ADMIN_DASHBOARD_ENDPOINTS.GET_STATE_COUNTS);
  }

  // Stub methods for export functionality
  exportRequests(state: DashboardRequestStatus, request: PaginationRequest<PaginationDashboardFilters>): Observable<Blob> {
    let params = new HttpParams()
      .set('state', state)
      .set('pageIndex', request.pageIndex.toString())
      .set('pageSize', request.pageSize.toString());

    if (request.searchString) {
      params = params.set('searchString', request.searchString);
    }

    if (request.sortColumn) {
      params = params.set('sortColumn', request.sortColumn);
    }

    if (request.sortDirection) {
      params = params.set('sortDirection', request.sortDirection);
    }

    if (request.filters.requestType !== undefined && request.filters.requestType !== null) {
      params = params.set('requestType', request.filters.requestType.toString());
    }

    if (request.filters.regionId !== undefined && request.filters.regionId !== null) {
      params = params.set('regionId', request.filters.regionId.toString());
    }

    if (request.filters.fromDate) {
      params = params.set('fromDate', request.filters.fromDate);
    }

    if (request.filters.toDate) {
      params = params.set('toDate', request.filters.toDate);
    }

    return this.http.get(ADMIN_DASHBOARD_ENDPOINTS.EXPORT_REQUESTS, { 
      params, 
      responseType: 'blob' 
    });
  }

  exportAllRequests(request: PaginationRequest<PaginationDashboardFilters>): Observable<Blob> {
    let params = new HttpParams()
      .set('pageIndex', request.pageIndex.toString())
      .set('pageSize', request.pageSize.toString());

    if (request.searchString) {
      params = params.set('searchString', request.searchString);
    }

    if (request.sortColumn) {
      params = params.set('sortColumn', request.sortColumn);
    }

    if (request.sortDirection) {
      params = params.set('sortDirection', request.sortDirection);
    }

    if (request.filters.requestType !== undefined && request.filters.requestType !== null) {
      params = params.set('requestType', request.filters.requestType.toString());
    }

    if (request.filters.regionId !== undefined && request.filters.regionId !== null) {
      params = params.set('regionId', request.filters.regionId.toString());
    }

    if (request.filters.fromDate) {
      params = params.set('fromDate', request.filters.fromDate);
    }

    if (request.filters.toDate) {
      params = params.set('toDate', request.filters.toDate);
    }

    return this.http.get(ADMIN_DASHBOARD_ENDPOINTS.EXPORT_ALL_REQUESTS, { 
      params, 
      responseType: 'blob' 
    });
  }
} 