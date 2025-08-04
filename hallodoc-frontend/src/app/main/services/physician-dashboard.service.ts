import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BASE_URL } from '@main/constants/api-endpoints';
import { 
  DashboardState, 
  AdminRequestData, 
  PaginationRequest, 
  PaginationResponse, 
  PaginationDashboardFilters 
} from '@main/interfaces';
import { DashboardRequestStatus } from '@main/enums';

@Injectable({
  providedIn: 'root'
})
export class PhysicianDashboardService {

  constructor(private http: HttpClient) { }

  getRequestsByState(state: DashboardRequestStatus, request: PaginationRequest<PaginationDashboardFilters>): Observable<PaginationResponse<AdminRequestData>> {
    let params = new HttpParams()
      .set('state', state.toString())
      .set('pageIndex', request.pageIndex.toString())
      .set('pageSize', request.pageSize.toString())
      .set('sortColumn', request.sortColumn || '')
      .set('sortDirection', request.sortDirection || 'asc')
      .set('searchString', request.searchString || '');

    if (request.filters) {
      if (request.filters.searchTerm) {
        params = params.set('filters.searchTerm', request.filters.searchTerm);
      }
      if (request.filters.requestType !== undefined && request.filters.requestType !== null) {
        params = params.set('filters.requestType', request.filters.requestType.toString());
      }
      if (request.filters.regionId !== undefined && request.filters.regionId !== null) {
        params = params.set('filters.regionId', request.filters.regionId.toString());
      }
      if (request.filters.fromDate) {
        params = params.set('filters.fromDate', request.filters.fromDate);
      }
      if (request.filters.toDate) {
        params = params.set('filters.toDate', request.filters.toDate);
      }
    }

    return this.http.get<PaginationResponse<AdminRequestData>>(`${BASE_URL}/api/physiciandashboard/requests`, { params });
  }

  getStateCounts(): Observable<DashboardState[]> {
    return this.http.get<DashboardState[]>(`${BASE_URL}/api/physiciandashboard/state-counts`);
  }

  exportRequests(state: DashboardRequestStatus, request: PaginationRequest<PaginationDashboardFilters>): Observable<Blob> {
    let params = new HttpParams()
      .set('state', state.toString())
      .set('pageIndex', request.pageIndex.toString())
      .set('pageSize', request.pageSize.toString())
      .set('sortColumn', request.sortColumn || '')
      .set('sortDirection', request.sortDirection || 'asc')
      .set('searchString', request.searchString || '');

    if (request.filters) {
      if (request.filters.searchTerm) {
        params = params.set('filters.searchTerm', request.filters.searchTerm);
      }
      if (request.filters.requestType !== undefined && request.filters.requestType !== null) {
        params = params.set('filters.requestType', request.filters.requestType.toString());
      }
      if (request.filters.regionId !== undefined && request.filters.regionId !== null) {
        params = params.set('filters.regionId', request.filters.regionId.toString());
      }
      if (request.filters.fromDate) {
        params = params.set('filters.fromDate', request.filters.fromDate);
      }
      if (request.filters.toDate) {
        params = params.set('filters.toDate', request.filters.toDate);
      }
    }

    return this.http.get(`${BASE_URL}/api/physiciandashboard/export`, { 
      params, 
      responseType: 'blob' 
    });
  }

  exportAllRequests(request: PaginationRequest<PaginationDashboardFilters>): Observable<Blob> {
    let params = new HttpParams()
      .set('pageIndex', request.pageIndex.toString())
      .set('pageSize', request.pageSize.toString())
      .set('sortColumn', request.sortColumn || '')
      .set('sortDirection', request.sortDirection || 'asc')
      .set('searchString', request.searchString || '');

    if (request.filters) {
      if (request.filters.searchTerm) {
        params = params.set('filters.searchTerm', request.filters.searchTerm);
      }
      if (request.filters.requestType !== undefined && request.filters.requestType !== null) {
        params = params.set('filters.requestType', request.filters.requestType.toString());
      }
      if (request.filters.regionId !== undefined && request.filters.regionId !== null) {
        params = params.set('filters.regionId', request.filters.regionId.toString());
      }
      if (request.filters.fromDate) {
        params = params.set('filters.fromDate', request.filters.fromDate);
      }
      if (request.filters.toDate) {
        params = params.set('filters.toDate', request.filters.toDate);
      }
    }

    return this.http.get(`${BASE_URL}/api/physiciandashboard/export-all`, { 
      params, 
      responseType: 'blob' 
    });
  }
} 