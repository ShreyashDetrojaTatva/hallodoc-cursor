export interface PaginationRequest<T = any> {
  pageIndex: number;
  pageSize: number;
  sortColumn?: string;
  sortDirection?: string; // 'asc' | 'desc'
  searchString?: string;
  filters: T;
} 