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
  selector: 'app-concierge-request-form',
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
  templateUrl: './concierge-request-form.component.html',
  styleUrls: ['./concierge-request-form.component.scss']
})
export class ConciergeRequestFormComponent {
  form: FormGroup;
  fileName: string = '';
  files: File[] = [];
  constructor(private fb: FormBuilder, private router: Router, private requestService: RequestService) {
    this.form = this.fb.group({
      conFirstName: ['', Validators.required],
      conLastName: ['', Validators.required],
      conPhone: ['', Validators.required],
      conEmail: ['', [Validators.required, Validators.email]],
      hotelName: ['', Validators.required],
      conStreet: ['', Validators.required],
      conCity: ['', Validators.required],
      conState: ['', Validators.required],
      conZipCode: ['', Validators.required],
      symptoms: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      dob: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', Validators.required],
      roomNo: [''],
      file: [null]
    });
  }

  onFileChange(event: any) {
    const selectedFiles = Array.from(event.target.files) as File[];
    this.files = selectedFiles;
    this.fileName = selectedFiles.map(f => f.name).join(', ');
    this.form.patchValue({ file: selectedFiles });
  }

  goBack() {
    this.router.navigate(['/request-type']);
  }

  onSubmit() {
    if (this.form.valid) {
      const dobValue = this.form.value.dob;
      const dobIso = dobValue instanceof Date ? dobValue.toISOString().split('T')[0] : dobValue;
      const data = {
        requestType: 3, // Concierge
        requestorType: 3, // Concierge
        requestorFirstName: this.form.value.conFirstName,
        requestorLastName: this.form.value.conLastName,
        requestorEmail: this.form.value.conEmail,
        requestorPhone: this.form.value.conPhone,
        hotelName: this.form.value.hotelName,
        propertyName: this.form.value.hotelName, // or another field if needed
        firstName: this.form.value.firstName,
        lastName: this.form.value.lastName,
        dob: dobIso,
        email: this.form.value.email,
        phone: this.form.value.phone,
        street: this.form.value.conStreet,
        city: this.form.value.conCity,
        state: this.form.value.conState,
        zipCode: this.form.value.conZipCode,
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