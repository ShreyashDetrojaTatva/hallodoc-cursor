import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';

import { AdminRequestService } from '@main/services/admin-request.service';
import { PhysicianData, AssignRequestData } from '@main/interfaces/admin/physician.interface';

@Component({
  selector: 'app-assign-request',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './assign-request.component.html',
  styleUrls: ['./assign-request.component.scss']
})
export class AssignRequestComponent implements OnInit {
  assignForm: FormGroup;
  physicians: PhysicianData[] = [];
  isLoading = false;
  error: string | null = null;
  currentPhysicianId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private adminRequestService: AdminRequestService,
    private dialogRef: MatDialogRef<AssignRequestComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { requestId: number; currentPhysicianId?: number }
  ) {
    this.assignForm = this.fb.group({
      physicianId: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.currentPhysicianId = this.data.currentPhysicianId || null;
    this.loadPhysicians();
  }

  loadPhysicians() {
    this.isLoading = true;
    this.error = null;

    this.adminRequestService.getPhysicians()
      .pipe(
        catchError(err => {
          this.error = 'Failed to load physicians. Please try again.';
          return of([]);
        }),
        finalize(() => this.isLoading = false)
      )
      .subscribe((physicians: PhysicianData[]) => {
        this.physicians = physicians;
        
        // Set current physician if exists
        if (this.currentPhysicianId) {
          this.assignForm.patchValue({
            physicianId: this.currentPhysicianId
          });
        }
      });
  }

  onSubmit() {
    if (this.assignForm.valid) {
      this.isLoading = true;
      this.error = null;

      const formData = this.assignForm.value;
      const assignData: AssignRequestData = {
        requestId: this.data.requestId,
        physicianId: formData.physicianId
      };

      this.adminRequestService.assignRequest(assignData)
        .pipe(
          catchError(err => {
            this.error = 'Failed to assign request. Please try again.';
            return of(null);
          }),
          finalize(() => this.isLoading = false)
        )
        .subscribe((result) => {
          if (result) {
            this.dialogRef.close({ success: true, physicianId: formData.physicianId });
          }
        });
    }
  }

  onCancel() {
    this.dialogRef.close({ success: false });
  }
} 