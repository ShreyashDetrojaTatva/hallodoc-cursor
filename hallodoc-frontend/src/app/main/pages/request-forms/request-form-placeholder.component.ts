import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-request-form-placeholder',
  standalone: true,
  imports: [CommonModule, MatCardModule],
  template: `
    <div style="display: flex; justify-content: center; align-items: center; min-height: 80vh;">
      <mat-card style="padding: 2rem; min-width: 320px; text-align: center;">
        <h2>Request Form Placeholder</h2>
        <p>Selected type: <strong>{{ type }}</strong></p>
        <p>Form coming soon...</p>
      </mat-card>
    </div>
  `
})
export class RequestFormPlaceholderComponent {
  type = '';
  constructor(private route: ActivatedRoute) {
    this.route.params.subscribe(params => {
      this.type = params['type'];
    });
  }
} 