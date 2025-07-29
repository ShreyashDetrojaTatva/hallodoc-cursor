import { Component, Inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { SafePipe } from '../../pipes/safe.pipe';
import { DocumentData } from '../../interfaces/document-viewer/document-data.interface';
import { DocumentViewerData } from '../../interfaces/document-viewer/document-viewer-data.interface';

@Component({
  selector: 'app-document-viewer',
  templateUrl: './document-viewer.component.html',
  styleUrls: ['./document-viewer.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatIconModule,
    MatButtonModule,
    SafePipe
  ]
})
export class DocumentViewerComponent implements OnDestroy {
  constructor(
    public dialogRef: MatDialogRef<DocumentViewerComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DocumentViewerData
  ) {}

  ngOnDestroy() {
    // Clean up blob URL when dialog is closed
    if (this.data.document.filePath && this.data.document.filePath.startsWith('blob:')) {
      window.URL.revokeObjectURL(this.data.document.filePath);
    }
  }

  close(): void {
    this.dialogRef.close();
  }

  download(): void {
    // Create a temporary link to download the file
    const link = document.createElement('a');
    link.href = this.data.document.filePath;
    link.download = this.data.document.fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  get displayTitle(): string {
    return this.data.title || this.data.document.fileName;
  }
} 