import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RequestService } from '../../../services/request/request.service';

@Component({
  selector: 'app-patient-request-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatIconModule,
    MatCardModule
  ],
  templateUrl: './patient-request-form.component.html',
  styleUrls: ['./patient-request-form.component.scss']
})
export class PatientRequestFormComponent {
  form: FormGroup;
  fileName: string = '';
  files: File[] = [];
  constructor(private fb: FormBuilder, private router: Router, private requestService: RequestService) {
    this.form = this.fb.group({
      symptoms: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      dob: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', Validators.required],
      street: ['', Validators.required],
      city: ['', Validators.required],
      state: ['', Validators.required],
      zipCode: ['', Validators.required],
      roomNo: [''],
      file: [null]
    });
  }

  onFileChange(event: Event) {
    const target = event.target as HTMLInputElement;
    if (target.files) {
      this.files = Array.from(target.files);
    }
  }

  goBack() {
    this.router.navigate(['/request-type']);
  }

  onSubmit() {
    if (this.form.valid) {
      const dobValue = this.form.value.dob;
      const dobIso = dobValue instanceof Date ? dobValue.toISOString().split('T')[0] : dobValue;
      const data = {
        requestType: 1, // Patient
        requestorType: 1, // Patient
        firstName: this.form.value.firstName,
        lastName: this.form.value.lastName,
        dob: dobIso,
        email: this.form.value.email,
        phone: this.form.value.phone,
        street: this.form.value.street,
        city: this.form.value.city,
        state: this.form.value.state,
        zipCode: this.form.value.zipCode,
        roomNo: this.form.value.roomNo,
        symptoms: this.form.value.symptoms,
        files: this.files
      };
      this.requestService.createRequest(data).subscribe({
        next: () => {
          alert('Request submitted successfully!');
          this.router.navigate(['/']);
        },
        error: () => {
          alert('Failed to submit request.');
        }
      });
    }
  }

  onCancel() {
    this.router.navigate(['/']);
  }
} 