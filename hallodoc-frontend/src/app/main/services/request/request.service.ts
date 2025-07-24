import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RequestService {
  private apiUrl = environment.baseUrl + '/api/request';

  constructor(private http: HttpClient) {}

  createRequest(data: any): Observable<any> {
    return this.http.post(this.apiUrl, data);
  }
} 