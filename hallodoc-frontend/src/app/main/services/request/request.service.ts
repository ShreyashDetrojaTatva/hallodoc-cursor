import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RequestService {
  private apiUrl = environment.baseUrl + '/api/request';

  constructor(private http: HttpClient) {}

  createRequest(data: any): Observable<any> {
    const formData = new FormData();
    // Append all fields except file
    Object.keys(data).forEach(key => {
      if (key !== 'file' && key !== 'files' && data[key] !== null && data[key] !== undefined) {
        formData.append(key, data[key]);
      }
    });
    // Append file if present
    if (data.file) {
      formData.append('Files', data.file);
    }
    // For future: support multiple files
    if (data.files && Array.isArray(data.files)) {
      data.files.forEach((f: File) => formData.append('Files', f));
    }
    return this.http.post(this.apiUrl, formData);
  }
} 