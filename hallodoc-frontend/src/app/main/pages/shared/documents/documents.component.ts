import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DialogWrapperService } from '@core/components';
import { AuthService, DocumentService } from '@main/services';
import { DocumentData, ProfileData } from '@main/interfaces';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';
import { AccountType } from '@main/enums';

@Component({
  selector: 'app-documents',
  templateUrl: './documents.component.html',
  styleUrls: ['./documents.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatTableModule,
    MatCheckboxModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ]
})
export class DocumentsComponent implements OnInit {
  requestId: number;
  documents: DocumentData[] = [];
  isLoading = false;
  error: string | null = null;
  selectedDocuments: Set<number> = new Set();
  isAllSelected = false;
  isAdmin = false;

  // Upload form
  uploadForm: FormGroup;
  isUploading = false;

  // Email form
  emailForm: FormGroup;
  showEmailDialog = false;

  displayedColumns: string[] = ['select', 'fileName', 'uploadedAt', 'actions'];
  user: ProfileData | null;

  constructor(
    private route: ActivatedRoute,
    private documentService: DocumentService,
    private dialogWrapper: DialogWrapperService,
    private formBuilder: FormBuilder,
    private snackBar: MatSnackBar,
    private router: Router,
    private authService: AuthService
  ) {
    const id = this.route.snapshot.paramMap.get('id');
    this.requestId = id ? parseInt(id, 10) : 0;
    
    this.user = this.authService.getCurrentUser();
    this.isAdmin = this.user?.accountType === AccountType.Admin;

    this.uploadForm = this.formBuilder.group({
      file: [null, Validators.required]
    });

    this.emailForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  ngOnInit() {
    this.loadDocuments();
  }

  loadDocuments() {
    if (!this.requestId) {
      this.error = 'No requestId provided';
      return;
    }

    this.isLoading = true;
    this.error = null;

    const serviceCall = this.isAdmin 
      ? this.documentService.getRequestDocumentsForAdmin(this.requestId)
      : this.documentService.getRequestDocumentsForPhysician(this.requestId);

    serviceCall
      .pipe(
        catchError(err => {
          console.error('Error loading documents:', err);
          this.error = 'Failed to load documents. Please try again.';
          return of([]);
        }),
        finalize(() => this.isLoading = false)
      )
      .subscribe(documents => {
        this.documents = documents as DocumentData[];
        this.updateSelectAllState();
      });
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.uploadForm.patchValue({ file });
    }
  }

  goBack() {
    this.router.navigate(['/admin/dashboard']);
  }

  uploadDocument() {
    if (this.uploadForm.valid && !this.isUploading) {
      this.isUploading = true;
      const formData = new FormData();
      formData.append('requestId', this.requestId.toString());
      formData.append('file', this.uploadForm.get('file')?.value);

      this.documentService.uploadDocument(formData)
        .pipe(
          catchError(err => {
            console.error('Error uploading document:', err);
            this.snackBar.open('Failed to upload document', 'Close', { duration: 3000 });
            return of(null);
          }),
          finalize(() => this.isUploading = false)
        )
        .subscribe(response => {
          if (response) {
            this.snackBar.open('Document uploaded successfully', 'Close', { duration: 3000 });
            this.uploadForm.reset();
            this.loadDocuments();
          }
        });
    }
  }

  downloadDocument(document: DocumentData) {
    this.documentService.downloadDocument(document.documentId, this.isAdmin)
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

  downloadSelectedDocuments() {
    if (this.selectedDocuments.size === 0) {
      this.snackBar.open('Please select documents to download', 'Close', { duration: 3000 });
      return;
    }

    this.documentService.downloadMultipleDocuments(Array.from(this.selectedDocuments), this.isAdmin)
      .subscribe(blob => {
        const url = window.URL.createObjectURL(blob);
        const link = window.document.createElement('a');
        link.href = url;
        link.download = 'documents.zip';
        window.document.body.appendChild(link);
        link.click();
        window.document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      });
  }

  viewDocument(document: DocumentData) {
    this.documentService.downloadDocument(document.documentId, this.isAdmin)
      .subscribe(blob => {
        const url = window.URL.createObjectURL(blob);
        const documentWithBlob = {
          ...document,
          filePath: url
        };
        
        this.dialogWrapper.openDocumentViewer(documentWithBlob);
      });
  }

  deleteDocument(document: DocumentData) {
    if (confirm(`Are you sure you want to delete "${document.fileName}"?`)) {
      this.documentService.deleteDocument(document.documentId, this.isAdmin)
        .subscribe(response => {
          if (response) {
            this.snackBar.open('Document deleted successfully', 'Close', { duration: 3000 });
            this.loadDocuments();
          } else {
            this.snackBar.open('Failed to delete document', 'Close', { duration: 3000 });
          }
        });
    }
  }

  emailDocuments() {
    if (this.selectedDocuments.size === 0) {
      this.snackBar.open('Please select documents to email', 'Close', { duration: 3000 });
      return;
    }

    this.showEmailDialog = true;
  }

  sendEmail() {
    if (this.emailForm.valid) {
      const emailData = {
        requestId: this.requestId,
        email: this.emailForm.get('email')?.value,
        documentIds: Array.from(this.selectedDocuments)
      };

      this.documentService.emailDocuments(emailData, this.isAdmin)
        .subscribe(response => {
          if (response) {
            this.snackBar.open('Documents sent successfully', 'Close', { duration: 3000 });
            this.showEmailDialog = false;
            this.emailForm.reset();
          } else {
            this.snackBar.open('Failed to send documents', 'Close', { duration: 3000 });
          }
        });
    }
  }

  // Selection methods
  toggleAllSelection() {
    if (this.isAllSelected) {
      this.selectedDocuments.clear();
    } else {
      this.documents.forEach(doc => this.selectedDocuments.add(doc.documentId));
    }
    this.updateSelectAllState();
  }

  toggleDocumentSelection(documentId: number) {
    if (this.selectedDocuments.has(documentId)) {
      this.selectedDocuments.delete(documentId);
    } else {
      this.selectedDocuments.add(documentId);
    }
    this.updateSelectAllState();
  }

  private updateSelectAllState() {
    this.isAllSelected = this.documents.length > 0 && 
                        this.documents.every(doc => this.selectedDocuments.has(doc.documentId));
  }

  refresh() {
    this.loadDocuments();
  }
} 