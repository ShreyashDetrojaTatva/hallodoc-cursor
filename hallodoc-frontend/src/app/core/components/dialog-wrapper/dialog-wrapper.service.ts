import { Injectable } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { DocumentViewerComponent } from '@main/components';
import { DocumentViewerData } from '@main/interfaces';
import { DialogConfig } from '@core/interfaces';

@Injectable({
  providedIn: 'root'
})
export class DialogWrapperService {
  constructor(private dialog: MatDialog) {}

  /**
   * Opens a document viewer dialog
   */
  openDocumentViewer(
    document: DocumentViewerData['document'],
    options?: {
      title?: string;
      showDownloadButton?: boolean;
      config?: DialogConfig;
    }
  ): Observable<unknown> {
    const data: DocumentViewerData = {
      document,
      title: options?.title,
      showDownloadButton: options?.showDownloadButton ?? true
    };

    const dialogConfig: MatDialogConfig = {
      data,
      width: options?.config?.width ?? '90vw',
      height: options?.config?.height ?? '90vh',
      maxWidth: options?.config?.maxWidth ?? '1200px',
      maxHeight: options?.config?.maxHeight ?? '800px',
      disableClose: options?.config?.disableClose ?? false,
      panelClass: options?.config?.panelClass
    };

    return this.dialog.open(DocumentViewerComponent, dialogConfig).afterClosed();
  }

  /**
   * Opens a generic dialog with custom component
   */
  openDialog<T>(
    component: any,
    data: unknown,
    config?: DialogConfig
  ): Observable<unknown> {
    const dialogConfig: MatDialogConfig = {
      data,
      width: config?.width ?? '500px',
      maxWidth: config?.maxWidth ?? '90vw',
      disableClose: config?.disableClose ?? false,
      panelClass: config?.panelClass
    };

    return this.dialog.open(component, dialogConfig).afterClosed();
  }

  /**
   * Opens a confirmation dialog
   */
  openConfirmationDialog(
    title: string,
    message: string,
    confirmText: string = 'Confirm',
    cancelText: string = 'Cancel'
  ): Observable<boolean> {
    // This would use a confirmation dialog component
    // For now, we'll use the browser's confirm dialog
    const confirmed = confirm(`${title}\n\n${message}`);
    return new Observable(observer => {
      observer.next(confirmed);
      observer.complete();
    });
  }

  /**
   * Closes all open dialogs
   */
  closeAllDialogs(): void {
    this.dialog.closeAll();
  }
} 