import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '@main/services';
import { AccountType } from '@main/enums';

@Injectable({
  providedIn: 'root'
})
export class AdminPhysicianGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(): boolean {
    const user = this.authService.getCurrentUser();
    
    if (!user) {
      this.router.navigate(['/login']);
      return false;
    }

    if (user.accountType !== AccountType.Admin && user.accountType !== AccountType.Physician) {
      // Redirect based on account type
      switch (user.accountType) {
        case AccountType.Patient:
          this.router.navigate(['/patient/dashboard']);
          break;
        default:
          this.router.navigate(['/login']);
          break;
      }
      return false;
    }

    return true;
  }
} 