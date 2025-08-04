import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

export interface AcceptRequestData {
  requestId: number;
  patientName: string;
}

@Component({
  selector: 'app-accept-request',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './accept-request.component.html',
  styleUrl: './accept-request.component.scss'
})
export class AcceptRequestComponent {
  isLoading = false;
  notes = '';

  constructor(
    public dialogRef: MatDialogRef<AcceptRequestComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AcceptRequestData
  ) {}

  onCancel(): void {
    this.dialogRef.close();
  }

  onAccept(): void {
    this.isLoading = true;
    this.dialogRef.close({
      success: true,
      requestId: this.data.requestId,
      notes: this.notes
    });
  }
} 