export interface PaginationResponse<T = any> {
  items: T[];
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  sortDirection: string;
  sortColumn: string;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
} 