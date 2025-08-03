import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';

import { AdminRequestService } from '@main/services/admin-request.service';
import { RequestDetailsData, UpdateRequestData } from '@main/interfaces/admin/request.interface';
import { RequestType } from '@main/enums/request-type.enum';
import { RequestStatus } from '@main/enums/request-status.enum';

@Component({
  selector: 'app-view-request',
  templateUrl: './view-request.component.html',
  styleUrls: ['./view-request.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatChipsModule
  ]
})
export class ViewRequestComponent implements OnInit {
  requestId: number = 0;
  requestData: RequestDetailsData | null = null;
  isLoading = false;
  error: string | null = null;
  isEditMode = false;
  
  requestForm: FormGroup;
  
  requestTypes = [
    { value: RequestType.Patient, label: 'Patient' },
    { value: RequestType.Family, label: 'Family/Friend' },
    { value: RequestType.Concierge, label: 'Concierge' },
    { value: RequestType.Business, label: 'Business' }
  ];
  
  requestStatuses = [
    { value: RequestStatus.Unassigned, label: 'Unassigned' },
    { value: RequestStatus.Accepted, label: 'Accepted' },
    { value: RequestStatus.Cancelled, label: 'Cancelled' },
    { value: RequestStatus.MDEnRoute, label: 'MD En Route' },
    { value: RequestStatus.MDONSite, label: 'MD On Site' },
    { value: RequestStatus.Conclude, label: 'Conclude' },
    { value: RequestStatus.CancelledByPatient, label: 'Cancelled By Patient' },
    { value: RequestStatus.Closed, label: 'Closed' },
    { value: RequestStatus.Unpaid, label: 'Unpaid' },
    { value: RequestStatus.Clear, label: 'Clear' },
    { value: RequestStatus.Blocked, label: 'Blocked' }
  ];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private adminRequestService: AdminRequestService,
    private fb: FormBuilder
  ) {
    this.requestForm = this.fb.group({
      // Patient Information
      patientFirstName: ['', Validators.required],
      patientLastName: ['', Validators.required],
      patientDOB: ['', Validators.required],
      patientPhone: ['', Validators.required],
      patientEmail: ['', [Validators.required, Validators.email]],
      patientStreet: ['', Validators.required],
      patientCity: ['', Validators.required],
      patientState: ['', Validators.required],
      patientZipCode: ['', Validators.required],
      
      // Request Information
      symptoms: ['', Validators.required],
      
      // Requestor Information
      requestorFirstName: [''],
      requestorLastName: [''],
      requestorPhone: [''],
      requestorEmail: [''],
      requestorRelation: ['']
    });
  }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    this.requestId = id ? parseInt(id, 10) : 0;
    
    if (this.requestId) {
      this.loadRequestDetails();
    }
  }

  loadRequestDetails() {
    this.isLoading = true;
    this.error = null;

    this.adminRequestService.getRequestDetails(this.requestId)
      .pipe(
        catchError(err => {
          this.error = 'Failed to load request details. Please try again.';
          return of(null);
        }),
        finalize(() => this.isLoading = false)
      )
      .subscribe((data: RequestDetailsData | null) => {
        if (data) {
          this.requestData = data;
          this.updateFormValidators();
          this.populateForm();
        }
      });
  }

  updateFormValidators() {
    if (this.requestData) {
      const isPatientRequest = this.requestData.requestType === RequestType.Patient;
      
      // Update requestor field validators based on request type
      const requestorFields = [
        'requestorFirstName',
        'requestorLastName', 
        'requestorPhone',
        'requestorEmail',
        'requestorRelation'
      ];

      requestorFields.forEach(field => {
        const control = this.requestForm.get(field);
        if (control) {
          if (isPatientRequest) {
            // Make fields optional for patient requests
            control.setValidators([]);
          } else {
            // Make fields required for non-patient requests
            const validators = field === 'requestorEmail' ? [Validators.required, Validators.email] : [Validators.required];
            control.setValidators(validators);
          }
          control.updateValueAndValidity();
        }
      });
    }
  }

  populateForm() {
    if (this.requestData) {
      this.requestForm.patchValue({
        // Patient Information
        patientFirstName: this.requestData.patientFirstName || '',
        patientLastName: this.requestData.patientLastName || '',
        patientDOB: this.requestData.patientDOB ? new Date(this.requestData.patientDOB) : '',
        patientPhone: this.requestData.patientPhone || '',
        patientEmail: this.requestData.patientEmail || '',
        patientStreet: this.requestData.patientStreet || '',
        patientCity: this.requestData.patientCity || '',
        patientState: this.requestData.patientState || '',
        patientZipCode: this.requestData.patientZipCode || '',
        
        // Request Information
        symptoms: this.requestData.symptoms || '',
        
        // Requestor Information
        requestorFirstName: this.requestData.requestorFirstName || '',
        requestorLastName: this.requestData.requestorLastName || '',
        requestorPhone: this.requestData.requestorPhone || '',
        requestorEmail: this.requestData.requestorEmail || '',
        requestorRelation: this.requestData.requestorRelation || ''
      });
    }
  }

  toggleEditMode() {
    this.isEditMode = !this.isEditMode;
    if (!this.isEditMode) {
      // Cancel edit - restore original data
      this.populateForm();
    }
  }

  saveRequest() {
    if (this.requestForm.valid) {
      this.isLoading = true;
      
      const formData = this.requestForm.value;
      const isPatientRequest = this.requestData?.requestType === RequestType.Patient;
      
      // Ensure date is in local format for timestamp without timezone
      const patientDOB = formData.patientDOB ? new Date(formData.patientDOB) : null;
      
      const updateData: UpdateRequestData = {
        requestId: this.requestId,
        // Patient Information
        patientFirstName: formData.patientFirstName,
        patientLastName: formData.patientLastName,
        patientDOB: patientDOB,
        patientPhone: formData.patientPhone,
        patientEmail: formData.patientEmail,
        patientStreet: formData.patientStreet,
        patientCity: formData.patientCity,
        patientState: formData.patientState,
        patientZipCode: formData.patientZipCode,
        
        // Request Information
        symptoms: formData.symptoms
      };

      // Only include requestor information for non-patient requests
      if (!isPatientRequest) {
        updateData.requestorFirstName = formData.requestorFirstName;
        updateData.requestorLastName = formData.requestorLastName;
        updateData.requestorPhone = formData.requestorPhone;
        updateData.requestorEmail = formData.requestorEmail;
        updateData.requestorRelation = formData.requestorRelation;
      }

      this.adminRequestService.updateRequest(updateData)
        .pipe(
          catchError(err => {
            console.error('Update request error:', err);
            this.error = 'Failed to update request. Please try again.';
            return of(null);
          }),
          finalize(() => this.isLoading = false)
        )
        .subscribe((result: any) => {
          if (result) {
            this.isEditMode = false;
            this.loadRequestDetails(); // Reload to get updated data
          }
        });
    }
  }

  viewDocuments() {
    this.router.navigate(['/admin/request', this.requestId, 'documents']);
  }

  viewNotes() {
    this.router.navigate(['/admin/request', this.requestId, 'notes']);
  }

  goBack() {
    this.router.navigate(['/admin/dashboard']);
  }

  getRequestTypeLabel(type: number): string {
    const typeOption = this.requestTypes.find(t => t.value === type);
    return typeOption ? typeOption.label : 'Unknown';
  }

  getRequestStatusLabel(status: number): string {
    const statusOption = this.requestStatuses.find(s => s.value === status);
    return statusOption ? statusOption.label : 'Unknown';
  }

  getStatusColor(status: number): string {
    switch (status) {
      case RequestStatus.Unassigned:
        return '#1976d2'; // Blue
      case RequestStatus.Accepted:
        return '#42a5f5'; // Light Blue
      case RequestStatus.MDEnRoute:
      case RequestStatus.MDONSite:
        return '#66bb6a'; // Green
      case RequestStatus.Conclude:
        return '#ec407a'; // Pink
      case RequestStatus.Closed:
      case RequestStatus.Cancelled:
      case RequestStatus.CancelledByPatient:
        return '#42a5f5'; // Light Blue
      case RequestStatus.Unpaid:
        return '#ab47bc'; // Purple
      default:
        return '#78909c'; // Grey
    }
  }
} 