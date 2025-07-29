import { Routes } from '@angular/router';
import { authGuard } from './main/guards/auth.guard';
import { LayoutComponent } from './main/components/layout/layout.component';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./main/pages/landing/landing.component').then(m => m.LandingComponent)
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
    path: 'request-type',
    loadComponent: () => import('./main/pages/request-type/request-type.component').then(m => m.RequestTypeComponent)
  },
  {
    path: 'request-form/:type',
    loadComponent: () => import('./main/pages/request-forms/request-form-placeholder.component').then(m => m.RequestFormPlaceholderComponent)
  },
  {
    path: 'patient',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./main/pages/patient/dashboard/patient-dashboard.component').then(m => m.PatientDashboardComponent)
      },
      {
        path: 'requests/:id/documents',
        loadComponent: () => import('./main/pages/patient/documents/patient-documents.component').then(m => m.PatientDocumentsComponent)
      },
      {
        path: 'profile',
        loadComponent: () => import('./main/pages/patient/profile/patient-profile.component').then(m => m.PatientProfileComponent)
      }
    ]
  }
];
