import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BASE_URL } from '@main/constants';
import { DocumentData } from '@main/interfaces';

export interface EmailDocumentsData {
  requestId: number;
  email: string;
  documentIds: number[];
}

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private readonly API_BASE = `${BASE_URL}/api/document`;

  constructor(private http: HttpClient) {}

  getRequestDocumentsForAdmin(requestId: number): Observable<DocumentData[]> {
    return this.http.get<DocumentData[]>(`${this.API_BASE}/admin/${requestId}`);
  }

  getRequestDocumentsForPhysician(requestId: number): Observable<DocumentData[]> {
    return this.http.get<DocumentData[]>(`${this.API_BASE}/physician/${requestId}`);
  }

  downloadDocument(documentId: number, isAdmin: boolean): Observable<Blob> {
    const params = new HttpParams().set('isAdmin', isAdmin.toString());
    return this.http.get(`${this.API_BASE}/download/${documentId}`, { 
      params, 
      responseType: 'blob' 
    });
  }

  downloadMultipleDocuments(documentIds: number[], isAdmin: boolean): Observable<Blob> {
    const params = new HttpParams()
      .set('documentIds', documentIds.join(','))
      .set('isAdmin', isAdmin.toString());
    return this.http.get(`${this.API_BASE}/download-multiple`, { 
      params, 
      responseType: 'blob' 
    });
  }

  uploadDocument(formData: FormData): Observable<any> {
    return this.http.post(`${this.API_BASE}/upload`, formData);
  }

  deleteDocument(documentId: number, isAdmin: boolean): Observable<any> {
    const params = new HttpParams().set('isAdmin', isAdmin.toString());
    return this.http.delete(`${this.API_BASE}/${documentId}`, { params });
  }

  emailDocuments(emailData: EmailDocumentsData, isAdmin: boolean): Observable<any> {
    const params = new HttpParams().set('isAdmin', isAdmin.toString());
    return this.http.post(`${this.API_BASE}/email`, emailData, { params });
  }
} 