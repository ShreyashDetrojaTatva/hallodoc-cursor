import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../../services/auth/auth.service';
import { Observable } from 'rxjs';
import { AccountType } from '../../../enums';

interface NavigationItem {
  label: string;
  route: string;
  icon: string;
  roles?: string[];
  accountTypes?: AccountType[];
}

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
    //   label: 'Admin Panel',
    //   route: '/admin/dashboard',
    //   icon: 'admin_panel_settings',
    //   accountTypes: [AccountType.Admin]
    // },
    // {
    //   label: 'Physician Dashboard',
    //   route: '/physician/dashboard',
    //   icon: 'medical_services',
    //   accountTypes: [AccountType.Physician]
    // }
  ];

  constructor(private authService: AuthService) {}

  get currentUser$(): Observable<any> {
    return this.authService.currentUser$;
  }

  // Get navigation items based on user role and account type
  getFilteredNavigationItems(user: any): NavigationItem[] {
    console.log('User object:', user);
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
        console.log('Checking accountType:', user.accountType, 'against:', item.accountTypes);
        if (item.accountTypes.includes(user.accountType)) {
          console.log('Match found for:', item.label);
          return true;
        }
      }
      
      return false;
    });
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