import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '@main/services';
import { AuthRouteData } from '@main/interfaces';
import { ProfileData } from '@main/interfaces';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const isLoggedIn = authService.isLoggedIn();
  
  // Check if route requires authentication
  const requiresAuth = (route.data as AuthRouteData)['requiresAuth'] ?? true;
  
  // Check if route should redirect to dashboard when logged in
  const redirectIfLoggedIn = (route.data as AuthRouteData)['redirectIfLoggedIn'] ?? false;

  // If route requires auth but user is not logged in
  if (requiresAuth && !isLoggedIn) {
    router.navigate(['/login']);
    return false;
  }

  // If route should redirect to dashboard when logged in and user is logged in
  if (redirectIfLoggedIn && isLoggedIn) {
    const currentUser = authService.getCurrentUser();
    if (currentUser) {
      switch (currentUser.accountType) {
        case 1: // Admin
          router.navigate(['/admin/dashboard']);
          break;
        case 2: // Physician
          router.navigate(['/physician/dashboard']);
          break;
        case 3: // Patient
          router.navigate(['/patient/dashboard']);
          break;
        default:
          router.navigate(['/patient/dashboard']); // Default fallback
      }
      return false;
    }
  }

  return true;
}; 