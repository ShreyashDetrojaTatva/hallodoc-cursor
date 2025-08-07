import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import {
  MatDialogRef,
  MAT_DIALOG_DATA,
  MatDialogModule,
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AgreementService } from '@main/services/agreement/agreement.service';
import { AdminRequestService } from '@main/services/admin-request.service';
import { AdminRequestData } from '@main/interfaces/admin';
import { RequestDetailsData } from '@main/interfaces/admin/request.interface';

@Component({
  selector: 'app-send-agreement-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './send-agreement-dialog.component.html',
  styleUrls: ['./send-agreement-dialog.component.scss'],
})
export class SendAgreementDialogComponent implements OnInit {
  agreementForm: FormGroup;
  isLoading = false;
  error: string | null = null;
  requestDetails: RequestDetailsData | null = null;

  constructor(
    private dialogRef: MatDialogRef<SendAgreementDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { request: AdminRequestData },
    private formBuilder: FormBuilder,
    private agreementService: AgreementService,
    private adminRequestService: AdminRequestService
  ) {
    this.agreementForm = this.formBuilder.group({
      patientEmail: ['', [Validators.required, Validators.email]],
    });
  }

  ngOnInit(): void {
    this.loadRequestDetails();
  }

  private loadRequestDetails(): void {
    this.isLoading = true;
    this.adminRequestService.getRequestDetails(this.data.request.id).subscribe({
      next: (details) => {
        this.requestDetails = details;
        this.agreementForm.patchValue({
          patientEmail: details.patientEmail,
        });
        this.isLoading = false;
      },
      error: (error) => {
        this.error = 'Failed to load request details';
        this.isLoading = false;
      },
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSend(): void {
    if (this.agreementForm.valid) {
      this.isLoading = true;
      this.error = null;

      const sendData = {
        requestId: this.data.request.id,
        patientEmail: this.agreementForm.get('patientEmail')?.value,
      };

      this.agreementService.sendAgreement(sendData).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.dialogRef.close({
            success: true,
            message: 'Agreement sent successfully',
          });
        },
        error: (error) => {
          this.isLoading = false;
          this.error = error.error?.message || 'Failed to send agreement';
        },
      });
    }
  }
}
