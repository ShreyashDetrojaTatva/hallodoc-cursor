import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';

import { AdminRequestService } from '@main/services/admin-request.service';
import { CancelRequestData } from '@main/interfaces/admin/cancel-request.interface';

@Component({
  selector: 'app-cancel-request',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './cancel-request.component.html',
  styleUrls: ['./cancel-request.component.scss']
})
export class CancelRequestComponent implements OnInit {
  cancelForm: FormGroup;
  isLoading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private adminRequestService: AdminRequestService,
    private dialogRef: MatDialogRef<CancelRequestComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { requestId: number }
  ) {
    this.cancelForm = this.fb.group({
      cancellationReason: ['', Validators.maxLength(500)]
    });
  }

  ngOnInit() {
    // Component initialization
  }

  onSubmit() {
    if (this.cancelForm.valid) {
      this.isLoading = true;
      this.error = null;

      const formData = this.cancelForm.value;
      const cancelData: CancelRequestData = {
        requestId: this.data.requestId,
        cancellationReason: formData.cancellationReason || undefined
      };

      this.adminRequestService.cancelRequest(cancelData)
        .pipe(
          catchError(err => {
            this.error = 'Failed to cancel request. Please try again.';
            return of(null);
          }),
          finalize(() => this.isLoading = false)
        )
        .subscribe((result) => {
          if (result) {
            this.dialogRef.close({ success: true, cancellationReason: formData.cancellationReason });
          }
        });
    }
  }

  onCancel() {
    this.dialogRef.close({ success: false });
  }
} 