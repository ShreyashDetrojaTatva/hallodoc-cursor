import { Routes } from '@angular/router';
import { AuthGuard } from './main/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./main/pages/landing/landing.component').then(m => m.LandingComponent)
  },
  {
    path: 'request-type',
    loadComponent: () => import('./main/pages/request-type/request-type.component').then(m => m.RequestTypeComponent)
  },
  {
    path: 'request-form/:type',
    loadComponent: () => import('./main/pages/request-forms/request-form-placeholder.component').then(m => m.RequestFormPlaceholderComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('./main/pages/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'forgot-password',
    loadComponent: () => import('./main/pages/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent)
  },
  {
    path: 'reset-password',
    loadComponent: () => import('./main/pages/auth/reset-password/reset-password.component').then(m => m.ResetPasswordComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./main/pages/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [AuthGuard]
  },
];
