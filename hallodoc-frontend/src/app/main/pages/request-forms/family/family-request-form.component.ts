import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { RequestService } from '@main/services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-family-request-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatIconModule,
    MatCardModule
  ],
  templateUrl: './family-request-form.component.html',
  styleUrls: ['./family-request-form.component.scss']
})
export class FamilyRequestFormComponent {
  form: FormGroup;
  fileName: string = '';
  files: File[] = [];
  constructor(private fb: FormBuilder, private router: Router, private requestService: RequestService) {
    this.form = this.fb.group({
      famFirstName: ['', Validators.required],
      famLastName: ['', Validators.required],
      famPhone: ['', Validators.required],
      famEmail: ['', [Validators.required, Validators.email]],
      famRelation: ['', Validators.required],
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
        requestType: 2, // Family
        requestorType: 2, // Family
        requestorFirstName: this.form.value.famFirstName,
        requestorLastName: this.form.value.famLastName,
        requestorEmail: this.form.value.famEmail,
        requestorPhone: this.form.value.famPhone,
        relationWithPatient: this.form.value.famRelation,
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