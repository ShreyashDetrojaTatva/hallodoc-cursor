import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { ProfileData } from '../../../main/interfaces/auth/profile-data.interface';

@Component({
  selector: 'app-shared-header',
  standalone: true,
  imports: [CommonModule, MatButtonModule],
  template: `
    <div class="header">
      <h1>Welcome to HalloDoc</h1>
      <div *ngIf="user">
        <p>Hello, {{ user.firstName }} {{ user.lastName }}</p>
        <p>Email: {{ user.email }}</p>
      </div>
    </div>
  `,
  styles: [`
    .header {
      padding: 1rem;
      background: #f5f5f5;
      border-bottom: 1px solid #ddd;
    }
  `]
})
export class SharedHeaderComponent {
  @Input() user: ProfileData | null = null;
} 