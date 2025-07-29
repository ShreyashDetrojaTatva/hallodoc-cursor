import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { tap } from 'rxjs/operators';
import { LoginResponse } from '@main/interfaces';
import { ProfileData } from '@main/interfaces';
import { ForgotPasswordResponse } from '@main/interfaces';
import { ResetPasswordResponse } from '@main/interfaces';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = `${environment.baseUrl}/api/auth`;
  private currentUserSubject = new BehaviorSubject<ProfileData | null>(null);
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

  getCurrentUser(): ProfileData | null {
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
          const updatedUser = {
            ...currentUser,
            ...data
          } as ProfileData;
          localStorage.setItem('user', JSON.stringify(updatedUser));
          this.currentUserSubject.next(updatedUser);
        })
      );
  }

  forgotPassword(email: string): Observable<ForgotPasswordResponse> {
    return this.http.post<ForgotPasswordResponse>(`${this.baseUrl}/forgot-password`, { email }).pipe(
      tap(response => {
        if (response && response.resetLink) {
        }
      })
    );
  }

  resetPassword(resetData: { token: string; password: string }): Observable<ResetPasswordResponse> {
    return this.http.post<ResetPasswordResponse>(`${this.baseUrl}/reset-password`, resetData);
  }
} 