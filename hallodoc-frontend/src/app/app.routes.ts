import { Routes } from '@angular/router';
import { authGuard } from './main/guards/auth.guard';
import { AdminGuard } from './main/guards/admin.guard';
import { PhysicianGuard } from './main/guards/physician.guard';
import { PatientGuard } from './main/guards/patient.guard';
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
  // Patient Routes - Protected by PatientGuard
  {
    path: 'patient',
    component: LayoutComponent,
    canActivate: [authGuard, PatientGuard],
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
  },
  // Admin Routes - Protected by AdminGuard
  {
    path: 'admin',
    component: LayoutComponent,
    canActivate: [authGuard, AdminGuard],
    data: { requiresAuth: true, redirectIfLoggedIn: false },
    children: [
      { 
        path: 'dashboard', 
        loadComponent: () => import('./main/pages/dashboard/dashboard.component').then(m => m.DashboardComponent), 
        data: { requiresAuth: true, redirectIfLoggedIn: false } 
      },
      { 
        path: 'request/:id/view', 
        loadComponent: () => import('./main/pages/admin/view-request/view-request.component').then(m => m.ViewRequestComponent), 
        data: { requiresAuth: true, redirectIfLoggedIn: false } 
      },
      { 
        path: 'request/:id/documents', 
        loadComponent: () => import('./main/pages/shared/documents/documents.component').then(m => m.DocumentsComponent), 
        data: { requiresAuth: true, redirectIfLoggedIn: false } 
      }
    ]
  },
  // Physician Routes - Protected by PhysicianGuard
  {
    path: 'physician',
    component: LayoutComponent,
    canActivate: [authGuard, PhysicianGuard],
    data: { requiresAuth: true, redirectIfLoggedIn: false },
    children: [
      { 
        path: 'dashboard', 
        loadComponent: () => import('./main/pages/physician/dashboard/physician-dashboard.component').then(m => m.PhysicianDashboardComponent), 
        data: { requiresAuth: true, redirectIfLoggedIn: false } 
      },
      { 
        path: 'request/:id/view', 
        loadComponent: () => import('./main/pages/admin/view-request/view-request.component').then(m => m.ViewRequestComponent), 
        data: { requiresAuth: true, redirectIfLoggedIn: false } 
      },
      { 
        path: 'request/:id/documents', 
        loadComponent: () => import('./main/pages/shared/documents/documents.component').then(m => m.DocumentsComponent), 
        data: { requiresAuth: true, redirectIfLoggedIn: false } 
      }
    ]
  },
  {
    path: 'request-forms',
    loadComponent: () => import('./main/pages/request-forms/request-form-placeholder.component').then(m => m.RequestFormPlaceholderComponent),
    canActivate: [authGuard],
    data: {
      requiresAuth: false,
      redirectIfLoggedIn: true
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
  }
];
