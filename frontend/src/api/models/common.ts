export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors: string | null;
}

export interface Pagination {
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface PaginatedData<T> {
  data: T[];
  pagination: Pagination;
}

export type PaginatedResponse<T> = ApiResponse<PaginatedData<T>>;