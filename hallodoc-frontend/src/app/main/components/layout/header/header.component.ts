import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '@main/services';
import { Observable } from 'rxjs';
import { AccountType } from '@main/enums';
import { NavigationItem } from '@main/interfaces';
import { ProfileData } from '@main/interfaces';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule
  ]
})
export class HeaderComponent {
  isDrawerOpen = false;

  // Navigation items - can be extended for future links
  navigationItems: NavigationItem[] = [
    {
      label: 'Dashboard',
      route: '/patient/dashboard',
      icon: 'dashboard',
      accountTypes: [AccountType.Patient]
    },
    {
      label: 'Profile',
      route: '/patient/profile',
      icon: 'person',
      accountTypes: [AccountType.Patient]
    },
    {
      label: 'Dashboard',
      route: '/admin/dashboard',
      icon: 'admin_panel_settings',
      accountTypes: [AccountType.Admin]
    }
    // Future navigation items can be added here
    // Example:
    // {
    //   label: 'Documents',
    //   route: '/patient/documents',
    //   icon: 'folder',
    //   accountTypes: [AccountType.Patient]
    // },
    // {
    //   label: 'Physician Dashboard',
    //   route: '/physician/dashboard',
    //   icon: 'medical_services',
    //   accountTypes: [AccountType.Physician]
    // }
  ];

  constructor(private authService: AuthService, private router: Router) {}

  get currentUser$(): Observable<ProfileData | null> {
    return this.authService.currentUser$;
  }

  // Get navigation items based on user role and account type
  getFilteredNavigationItems(user: ProfileData | null): NavigationItem[] {
    if (!user) return [];
    
    return this.navigationItems.filter(item => {
      // If no role/account type restrictions, show to all
      if (!item.roles && !item.accountTypes) return true;
      
      // Check role-based access (for future custom roles)
      if (item.roles && user.roleId) {
        if (item.roles.includes(user.roleId.toString())) return true;
      }
      
      // Check account type-based access
      if (item.accountTypes && user.accountType) {
        if (item.accountTypes.includes(user.accountType)) {
          return true;
        }
      }
      
      return false;
    });
  }

  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  onLogout() {
    this.authService.logout();
    this.closeDrawer();
  }

  toggleDrawer() {
    this.isDrawerOpen = !this.isDrawerOpen;
  }

  closeDrawer() {
    this.isDrawerOpen = false;
  }

  onNavClick() {
    this.closeDrawer();
  }
} 