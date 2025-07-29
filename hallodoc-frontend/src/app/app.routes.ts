import { Routes } from '@angular/router';
import { authGuard } from './main/guards/auth.guard';
import { LayoutComponent } from './main/components/layout/layout.component';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./main/pages/landing/landing.component').then(m => m.LandingComponent),
    canActivate: [authGuard],
    data: { 
      requiresAuth: false,
      redirectIfLoggedIn: false 
    }
  },
  {
    path: 'login',
    loadComponent: () => import('./main/pages/auth/login/login.component').then(m => m.LoginComponent),
    canActivate: [authGuard],
    data: { 
      requiresAuth: false,
      redirectIfLoggedIn: true 
    }
  },
  {
    path: 'forgot-password',
    loadComponent: () => import('./main/pages/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent),
    canActivate: [authGuard],
    data: { 
      requiresAuth: false,
      redirectIfLoggedIn: true 
    }
  },
  {
    path: 'reset-password',
    loadComponent: () => import('./main/pages/auth/reset-password/reset-password.component').then(m => m.ResetPasswordComponent),
    canActivate: [authGuard],
    data: { 
      requiresAuth: false,
      redirectIfLoggedIn: true 
    }
  },
  {
    path: 'request-type',
    loadComponent: () => import('./main/pages/request-type/request-type.component').then(m => m.RequestTypeComponent),
    canActivate: [authGuard],
    data: { 
      requiresAuth: false,
      redirectIfLoggedIn: true 
    }
  },
  {
    path: 'request-form/:type',
    loadComponent: () => import('./main/pages/request-forms/request-form-placeholder.component').then(m => m.RequestFormPlaceholderComponent),
    canActivate: [authGuard],
    data: { 
      requiresAuth: false,
      redirectIfLoggedIn: true 
    }
  },
  {
    path: 'patient',
    component: LayoutComponent,
    canActivate: [authGuard],
    data: { 
      requiresAuth: true,
      redirectIfLoggedIn: false 
    },
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./main/pages/patient/dashboard/patient-dashboard.component').then(m => m.PatientDashboardComponent),
        data: { 
          requiresAuth: true,
          redirectIfLoggedIn: false 
        }
      },
      {
        path: 'requests/:id/documents',
        loadComponent: () => import('./main/pages/patient/documents/patient-documents.component').then(m => m.PatientDocumentsComponent),
        data: { 
          requiresAuth: true,
          redirectIfLoggedIn: false 
        }
      },
      {
        path: 'profile',
        loadComponent: () => import('./main/pages/patient/profile/patient-profile.component').then(m => m.PatientProfileComponent),
        data: { 
          requiresAuth: true,
          redirectIfLoggedIn: false 
        }
      }
    ]
  }
];
