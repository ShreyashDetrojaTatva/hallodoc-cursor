import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { Router } from '@angular/router';

interface LoginResponse {
  token: string;
  user: any;
}

interface ProfileData {
  firstName: string;
  lastName: string;
  email: string;
  username: string;
  phoneNumber: string;
  dob: Date;
  address: string;
  city: string;
  regionId: number;
  zipCode: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = `${environment.baseUrl}/api/auth`;
  private currentUserSubject = new BehaviorSubject<any>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.loadCurrentUser();
  }

  private loadCurrentUser() {
    const user = localStorage.getItem('user');
    if (user) {
      this.currentUserSubject.next(JSON.parse(user));
    }
  }

  login(usernameOrEmail: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, { usernameOrEmail, password })
      .pipe(
        tap(response => {
          localStorage.setItem('token', response.token);
          localStorage.setItem('user', JSON.stringify(response.user));
          this.currentUserSubject.next(response.user);
        })
      );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getCurrentUser(): any {
    return this.currentUserSubject.value;
  }

  getProfile(): Observable<ProfileData> {
    return this.http.get<ProfileData>(`${this.baseUrl}/profile`);
  }

  updateProfile(data: Partial<ProfileData>): Observable<ProfileData> {
    return this.http.put<ProfileData>(`${this.baseUrl}/profile`, data)
      .pipe(
        tap(() => {
          const currentUser = this.getCurrentUser();
          // Use the form data since backend doesn't return updated profile
          const updatedUser = {
            ...currentUser,
            ...data
          };
          localStorage.setItem('user', JSON.stringify(updatedUser));
          this.currentUserSubject.next(updatedUser);
        })
      );
  }

  forgotPassword(email: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/forgot-password`, { email }).pipe(
      tap(response => {
        if (response && (response as any).resetLink) {
          console.log('Reset Password Link:', (response as any).resetLink);
        }
      })
    );
  }

  resetPassword(resetData: { token: string; password: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/reset-password`, resetData);
  }
} 