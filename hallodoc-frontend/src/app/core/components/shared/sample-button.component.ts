import { Component, Input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-sample-button',
  standalone: true,
  imports: [MatButtonModule],
  template: `<button mat-raised-button color="accent">{{label}}</button>`
})
export class SampleButtonComponent {
  @Input() label = 'Sample Button';
} 