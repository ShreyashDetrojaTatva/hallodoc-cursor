import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Router, ActivatedRoute } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent {
  form: FormGroup;
  loading = false;
  message = '';
  token = '';

  constructor(
    private fb: FormBuilder, 
    public router: Router, 
    private route: ActivatedRoute,
    private auth: AuthService
  ) {
    this.form = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });

    // Get token from URL query parameter
    this.route.queryParams.subscribe(params => {
      this.token = params['token'] || '';
      if (!this.token) {
        this.message = 'Invalid reset link. Please request a new password reset.';
      }
    });
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  onSubmit() {
    if (this.form.valid && this.token) {
      this.loading = true;
      const resetData = {
        token: this.token,
        password: this.form.value.password
      };
      
      this.auth.resetPassword(resetData)
        .subscribe({
          next: () => {
            this.message = 'Password reset successfully. You can now login with your new password.';
            this.loading = false;
            setTimeout(() => {
              this.router.navigate(['/login']);
            }, 2000);
          },
          error: () => {
            this.message = 'An error occurred. Please try again.';
            this.loading = false;
          }
        });
    }
  }
} 