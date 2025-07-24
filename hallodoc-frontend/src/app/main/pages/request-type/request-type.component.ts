import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { MatIconModule } from "@angular/material/icon";

@Component({
  selector: 'app-request-type',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatIconModule],
  templateUrl: './request-type.component.html',
  styleUrls: ['./request-type.component.scss']
})
export class RequestTypeComponent {
  requestTypes = [
    { label: 'Patient', value: 'patient' },
    { label: 'Family/Friend', value: 'family' },
    { label: 'Concierge', value: 'concierge' },
    { label: 'Business Partners', value: 'business' }
  ];

  constructor(private router: Router) {}

  selectType(type: string) {
    this.router.navigate(['/request-form', type]);
  }

  goBack() {
    this.router.navigate(['']);
  }
} 