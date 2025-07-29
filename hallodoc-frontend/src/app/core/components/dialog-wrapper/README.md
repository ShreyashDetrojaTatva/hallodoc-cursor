# Dialog Wrapper Service

A generic, reusable service for managing dialogs that can be used across any project. This service provides common dialog operations and configurations.

## Usage Examples

### 1. Document Viewer Dialog (Project-Specific)

```typescript
import { DialogWrapperService } from '../../../core/components/dialog-wrapper/dialog-wrapper.service';

constructor(private dialogWrapper: DialogWrapperService) {}

viewDocument(document: DocumentData) {
  this.requestService.downloadDocument(document.documentId)
    .subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const documentWithBlob = { ...document, filePath: url };
      
      this.dialogWrapper.openDocumentViewer(documentWithBlob, {
        title: 'View Document',
        showDownloadButton: true
      });
    });
}
```

### 2. Generic Dialog

```typescript
import { DialogWrapperService } from '../../../core/components/dialog-wrapper/dialog-wrapper.service';
import { MyCustomComponent } from './my-custom.component';

constructor(private dialogWrapper: DialogWrapperService) {}

openCustomDialog() {
  this.dialogWrapper.openDialog(MyCustomComponent, {
    title: 'Custom Dialog',
    data: { someData: 'value' }
  }, {
    width: '600px',
    maxWidth: '90vw'
  });
}
```

### 3. Confirmation Dialog

```typescript
import { DialogWrapperService } from '../../../core/components/dialog-wrapper/dialog-wrapper.service';

constructor(private dialogWrapper: DialogWrapperService) {}

confirmAction() {
  this.dialogWrapper.openConfirmationDialog(
    'Confirm Action',
    'Are you sure you want to proceed?',
    'Yes, Proceed',
    'Cancel'
  ).subscribe(confirmed => {
    if (confirmed) {
      // Perform action
    }
  });
}
```

## Available Methods

- `openDocumentViewer()` - Opens document viewer dialog
- `openDialog()` - Opens generic dialog with custom component
- `openConfirmationDialog()` - Opens confirmation dialog
- `closeAllDialogs()` - Closes all open dialogs

## Configuration Options

- `width` - Dialog width (default: '500px')
- `height` - Dialog height (default: auto)
- `maxWidth` - Maximum width (default: '90vw')
- `maxHeight` - Maximum height (default: auto)
- `disableClose` - Prevent closing with ESC/click outside (default: false)
- `panelClass` - Custom CSS classes for styling 