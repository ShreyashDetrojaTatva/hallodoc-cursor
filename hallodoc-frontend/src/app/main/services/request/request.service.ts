import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { RequestData } from '../../interfaces/request/request-data.interface';
import { RequestFilter } from '../../interfaces/request/request-filter.interface';

@Injectable({
  providedIn: 'root'
})
export class RequestService {
  private baseUrl = `${environment.baseUrl}/api/request`;

  constructor(private http: HttpClient) { }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An error occurred';
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = error.error.message;
    } else {
      // Server-side error
      if (error.status === 401) {
        errorMessage = 'Please log in to access this feature';
      } else if (error.error?.message) {
        errorMessage = error.error.message;
      } else {
        errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
      }
    }
    return throwError(() => new Error(errorMessage));
  }

  createRequest(data: RequestData): Observable<unknown> {
    const formData = new FormData();
    Object.keys(data).forEach(key => {
      if (key === 'files' && data.files) {
        data.files.forEach(file => {
          formData.append('files', file);
        });
      } else if (key === 'dob') {
        // Handle date conversion safely
        const dobValue = data[key];
        if (dobValue instanceof Date) {
          formData.append(key, dobValue.toISOString().split('T')[0]);
        } else if (typeof dobValue === 'string') {
          formData.append(key, dobValue);
        } else {
          formData.append(key, String(data[key as keyof RequestData]));
        }
      } else {
        formData.append(key, (data as any)[key]);
      }
    });
    return this.http.post(`${this.baseUrl}`, formData)
      .pipe(catchError(this.handleError));
  }

  getPatientRequests(filter?: RequestFilter): Observable<unknown> {
    let params = {};
    if (filter) {
      params = {
        ...filter,
        startDate: filter.startDate?.toISOString(),
        endDate: filter.endDate?.toISOString()
      };
    }
    return this.http.get(`${this.baseUrl}/patient`, { params })
      .pipe(catchError(this.handleError));
  }

  getRequestDocuments(requestId: number): Observable<unknown> {
    return this.http.get(`${this.baseUrl}/${requestId}/documents`)
      .pipe(catchError(this.handleError));
  }

  downloadDocument(documentId: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/documents/${documentId}/download`, { responseType: 'blob' })
      .pipe(catchError(this.handleError));
  }
} 