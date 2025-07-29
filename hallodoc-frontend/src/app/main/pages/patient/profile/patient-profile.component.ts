import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { AuthService } from '@main/services';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';
import { ProfileData } from '@main/interfaces';
import { Region } from '@main/interfaces';

@Component({
  selector: 'app-patient-profile',
  templateUrl: './patient-profile.component.html',
  styleUrls: ['./patient-profile.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatIconModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatDatepickerModule,
    MatNativeDateModule
  ]
})
export class PatientProfileComponent implements OnInit {
  profileForm: FormGroup;
  isLoading = false;
  error: string | null = null;
  successMessage: string | null = null;
  regions: Region[] = [
    { id: 1, name: 'Region 1' },
    { id: 2, name: 'Region 2' },
    { id: 3, name: 'Region 3' }
    // Add more regions as needed
  ];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService
  ) {
    this.profileForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      email: [{ value: '', disabled: true }],
      username: [{ value: '', disabled: true }],
      phoneNumber: ['', [Validators.pattern('^[0-9]{10}$')]],
      dob: ['', Validators.required],
      address: ['', Validators.maxLength(255)],
      city: ['', Validators.maxLength(100)],
      regionId: [''],
      zipCode: ['', [Validators.pattern('^[0-9]{5}$')]]
    });
  }

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.isLoading = true;
    this.error = null;

    this.authService.getProfile()
      .pipe(
        catchError(err => {
          this.error = 'Failed to load profile. Please try again.';
          return of(null);
        }),
        finalize(() => this.isLoading = false)
      )
      .subscribe(profile => {
        if (profile) {
          this.profileForm.patchValue({
            ...profile,
            dob: new Date(profile.dob)
          });
        }
      });
  }

  onSubmit() {
    // Check only enabled fields for validation
    const enabledControls = Object.keys(this.profileForm.controls).filter(key => 
      !this.profileForm.get(key)?.disabled
    );
    
    const hasErrors = enabledControls.some(key => {
      const control = this.profileForm.get(key);
      return control && control.invalid && control.touched;
    });
    
    if (hasErrors) return;

    this.isLoading = true;
    this.error = null;
    this.successMessage = null;

    const formValue = this.profileForm.getRawValue();
    // Convert dob to YYYY-MM-DD (local, not UTC)
    if (formValue.dob instanceof Date) {
      formValue.dob = formValue.dob.getFullYear() + '-' +
        String(formValue.dob.getMonth() + 1).padStart(2, '0') + '-' +
        String(formValue.dob.getDate()).padStart(2, '0');
    }
    this.authService.updateProfile(formValue)
      .pipe(
        catchError(err => {
          this.error = 'Failed to update profile. Please try again.';
          return of(null);
        }),
        finalize(() => this.isLoading = false)
      )
      .subscribe(response => {
        if (response) {
          this.successMessage = 'Profile updated successfully.';
        }
      });
  }

  getErrorMessage(controlName: string): string {
    const control = this.profileForm.get(controlName);
    if (!control) return '';

    if (control.hasError('required')) {
      return 'This field is required';
    }
    if (control.hasError('maxlength')) {
      return `Maximum length is ${control.errors?.['maxlength'].requiredLength} characters`;
    }
    if (control.hasError('pattern')) {
      switch (controlName) {
        case 'phoneNumber':
          return 'Please enter a valid 10-digit phone number';
        case 'zipCode':
          return 'Please enter a valid 5-digit zip code';
        default:
          return 'Invalid format';
      }
    }
    return '';
  }

  isFormValid(): boolean {
    // Check only enabled fields for validation
    const enabledControls = Object.keys(this.profileForm.controls).filter(key => 
      !this.profileForm.get(key)?.disabled
    );
    
    return !enabledControls.some(key => {
      const control = this.profileForm.get(key);
      return control && control.invalid && control.touched;
    });
  }
} 