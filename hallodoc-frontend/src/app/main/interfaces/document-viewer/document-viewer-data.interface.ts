import { DocumentData } from './document-data.interface';

export interface DocumentViewerData {
  document: DocumentData;
  title?: string;
  showDownloadButton?: boolean;
} 