import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { DialogWrapperService } from '@core/components';
import { RequestService } from '@main/services';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';
import { DocumentData } from '@main/interfaces';

@Component({
  selector: 'app-patient-documents',
  templateUrl: './patient-documents.component.html',
  styleUrls: ['./patient-documents.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ]
})
export class PatientDocumentsComponent implements OnInit {
  requestId: number;
  documents: DocumentData[] = [];
  isLoading = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private requestService: RequestService,
    private dialogWrapper: DialogWrapperService
  ) {
    const id = this.route.snapshot.paramMap.get('id');
    this.requestId = id ? parseInt(id, 10) : 0;
  }

  ngOnInit() {
    console.log('PatientDocumentsComponent initialized with requestId:', this.requestId);
    this.loadDocuments();
  }

  loadDocuments() {
    if (!this.requestId) {
      console.error('No requestId provided');
      return;
    }

    console.log('Loading documents for requestId:', this.requestId);
    this.isLoading = true;
    this.error = null;

    this.requestService.getRequestDocuments(this.requestId)
      .pipe(
        catchError(err => {
          console.error('Error loading documents:', err);
          this.error = 'Failed to load documents. Please try again.';
          return of([]);
        }),
        finalize(() => this.isLoading = false)
      )
      .subscribe(documents => {
        console.log('Documents loaded:', documents);
        this.documents = documents as DocumentData[];
      });
  }

  downloadDocument(document: DocumentData) {
    this.requestService.downloadDocument(this.requestId, document.documentId)
      .subscribe(blob => {
        const url = window.URL.createObjectURL(blob);
        const link = window.document.createElement('a');
        link.href = url;
        link.download = document.fileName;
        window.document.body.appendChild(link);
        link.click();
        window.document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      });
  }

  viewDocument(document: DocumentData) {
    // Create a blob URL for viewing the document
    this.requestService.downloadDocument(this.requestId, document.documentId)
      .subscribe(blob => {
        const url = window.URL.createObjectURL(blob);
        const documentWithBlob = {
          ...document,
          filePath: url
        };
        
        // Open document in dialog using the wrapper service
        this.dialogWrapper.openDocumentViewer(documentWithBlob, {
          showDownloadButton: true
        });
      });
  }

  refresh() {
    this.loadDocuments();
  }
} 