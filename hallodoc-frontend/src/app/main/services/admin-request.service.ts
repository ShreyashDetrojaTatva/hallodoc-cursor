import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ADMIN_REQUEST_ENDPOINTS } from '@main/constants/api-endpoints';
import { RequestDetailsData, UpdateRequestData } from '@main/interfaces/admin/request.interface';
import { PhysicianData, AssignRequestData } from '@main/interfaces/admin/physician.interface';

@Injectable({
  providedIn: 'root'
})
export class AdminRequestService {

  constructor(private http: HttpClient) { }

  getRequestDetails(requestId: number): Observable<RequestDetailsData> {
    return this.http.get<RequestDetailsData>(ADMIN_REQUEST_ENDPOINTS.GET_REQUEST_DETAILS(requestId));
  }

  updateRequest(data: UpdateRequestData): Observable<any> {
    return this.http.put(ADMIN_REQUEST_ENDPOINTS.UPDATE_REQUEST, data);
  }

  getPhysicians(): Observable<PhysicianData[]> {
    return this.http.get<PhysicianData[]>(ADMIN_REQUEST_ENDPOINTS.GET_PHYSICIANS);
  }

  assignRequest(data: AssignRequestData): Observable<any> {
    return this.http.post(ADMIN_REQUEST_ENDPOINTS.ASSIGN_REQUEST, data);
  }
} 