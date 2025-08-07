import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import {
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AgreementService } from '@main/services/agreement/agreement.service';
import { AgreementDetailsData } from '@main/interfaces/agreement';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Inject } from '@angular/core';

@Component({
  selector: 'app-agreement',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  templateUrl: './agreement.component.html',
  styleUrls: ['./agreement.component.scss'],
})
export class AgreementComponent implements OnInit {
  token: string = '';
  agreementDetails: AgreementDetailsData | null = null;
  isLoading = true;
  error: string | null = null;
  isProcessing = false;
  cancellationForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private agreementService: AgreementService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private formBuilder: FormBuilder
  ) {
    this.cancellationForm = this.formBuilder.group({
      reason: ['', [Validators.required, Validators.minLength(10)]],
    });
  }

  ngOnInit(): void {
    this.token = this.route.snapshot.params['token'];
    if (!this.token) {
      this.error = 'Invalid agreement link';
      this.isLoading = false;
      return;
    }

    this.loadAgreementDetails();
  }

  private loadAgreementDetails(): void {
    this.agreementService.getAgreementDetails(this.token).subscribe({
      next: (details) => {
        this.agreementDetails = details;
        this.isLoading = false;
      },
      error: (error) => {
        this.error = error.error?.message || 'Failed to load agreement details';
        this.isLoading = false;
      },
    });
  }

  onAccept(): void {
    if (this.agreementDetails?.isExpired || this.agreementDetails?.isUsed) {
      return;
    }

    this.isProcessing = true;
    this.agreementService
      .processAgreementResponse({
        token: this.token,
        isAccepted: true,
      })
      .subscribe({
        next: (response) => {
          this.isProcessing = false;
          this.snackBar.open('Agreement accepted successfully!', 'Close', {
            duration: 5000,
          });
          this.router.navigate(['/']);
        },
        error: (error) => {
          this.isProcessing = false;
          this.error = error.error?.message || 'Failed to accept agreement';
        },
      });
  }

  onCancel(): void {
    if (this.agreementDetails?.isExpired || this.agreementDetails?.isUsed) {
      return;
    }

    const dialogRef = this.dialog.open(CancellationDialogComponent, {
      width: '400px',
      data: { form: this.cancellationForm },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.processCancellation(result.reason);
      }
    });
  }

  private processCancellation(reason: string): void {
    this.isProcessing = true;
    this.agreementService
      .processAgreementResponse({
        token: this.token,
        isAccepted: false,
        cancellationReason: reason,
      })
      .subscribe({
        next: (response) => {
          this.isProcessing = false;
          this.snackBar.open('Agreement cancelled successfully!', 'Close', {
            duration: 5000,
          });
          this.router.navigate(['/']);
        },
        error: (error) => {
          this.isProcessing = false;
          this.error = error.error?.message || 'Failed to cancel agreement';
        },
      });
  }

  get canRespond(): boolean {
    return !this.agreementDetails?.isExpired && !this.agreementDetails?.isUsed;
  }
}

@Component({
  selector: 'app-cancellation-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
  template: `
    <div class="cancellation-dialog">
      <h2 mat-dialog-title>Cancel Agreement</h2>
      <mat-dialog-content>
        <p>Please provide a reason for cancelling this agreement:</p>
        <form [formGroup]="form">
          <mat-form-field appearance="outline" class="full-width">
            <mat-label>Reason for Cancellation</mat-label>
            <textarea
              matInput
              formControlName="reason"
              rows="4"
              placeholder="Please explain why you are cancelling this agreement..."
            ></textarea>
            <mat-error *ngIf="form.get('reason')?.hasError('required')">
              Reason is required
            </mat-error>
            <mat-error *ngIf="form.get('reason')?.hasError('minlength')">
              Reason must be at least 10 characters
            </mat-error>
          </mat-form-field>
        </form>
      </mat-dialog-content>
      <mat-dialog-actions align="end">
        <button mat-button (click)="onCancel()">Cancel</button>
        <button
          mat-raised-button
          color="warn"
          (click)="onConfirm()"
          [disabled]="!form.valid"
        >
          Confirm Cancellation
        </button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [
    `
      .cancellation-dialog {
        min-width: 400px;
      }
      .full-width {
        width: 100%;
      }
    `,
  ],
})
export class CancellationDialogComponent {
  constructor(
    private dialogRef: MatDialogRef<CancellationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { form: FormGroup }
  ) {}

  get form(): FormGroup {
    return this.data.form;
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onConfirm(): void {
    if (this.form.valid) {
      this.dialogRef.close({ reason: this.form.get('reason')?.value });
    }
  }
}
