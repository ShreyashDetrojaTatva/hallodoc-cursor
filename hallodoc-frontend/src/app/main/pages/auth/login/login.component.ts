import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { AuthService } from '@main/services';
import { AccountType } from '@main/enums';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatCardModule
  ]
})
export class LoginComponent {
  form: FormGroup;
  isLoading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router
  ) {
    this.form = this.fb.group({
      usernameOrEmail: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.isLoading = true;
    this.error = null;

    this.auth.login(
      this.form.value.usernameOrEmail,
      this.form.value.password
    ).subscribe({
      next: (response) => {
        // Redirect based on account type
        switch (response.user.accountType) {
          case AccountType.Admin:
            this.router.navigate(['/admin/dashboard']);
            break;
          case AccountType.Physician:
            this.router.navigate(['/physician/dashboard']);
            break;
          case AccountType.Patient:
            this.router.navigate(['/patient/dashboard']);
            break;
          default:
            // Fallback to patient dashboard for unknown account types
            this.router.navigate(['/patient/dashboard']);
            break;
        }
      },
      error: (error) => {
        this.error = 'Invalid username/email or password';
        this.isLoading = false;
      }
    });
  }
} 