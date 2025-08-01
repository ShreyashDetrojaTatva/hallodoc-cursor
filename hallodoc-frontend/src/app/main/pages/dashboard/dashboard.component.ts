import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Subject } from 'rxjs';
import { AuthService } from '@main/services';
import { 
  DashboardState, 
  AdminRequestData, 
  RequestAction 
} from '@main/interfaces';
import { PaginationRequest, PaginationResponse, PaginationDashboardFilters } from '@main/interfaces/pagination';
import { DashboardRequestStatus, RequestType } from '@main/enums';
import { RequestStatusMapper } from '@main/utils/request-status-mapper';
import { AdminDashboardService } from '@main/services/admin-dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatMenuModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  // Dashboard states
  dashboardStates: DashboardState[] = [
    { id: DashboardRequestStatus.New, name: RequestStatusMapper.getDashboardStatusDisplayName(DashboardRequestStatus.New), count: 0, color: '#1976d2', icon: 'settings' },
    { id: DashboardRequestStatus.Pending, name: RequestStatusMapper.getDashboardStatusDisplayName(DashboardRequestStatus.Pending), count: 0, color: '#42a5f5', icon: 'notifications' },
    { id: DashboardRequestStatus.Active, name: RequestStatusMapper.getDashboardStatusDisplayName(DashboardRequestStatus.Active), count: 0, color: '#66bb6a', icon: 'check_circle' },
    { id: DashboardRequestStatus.Conclude, name: RequestStatusMapper.getDashboardStatusDisplayName(DashboardRequestStatus.Conclude), count: 0, color: '#ec407a', icon: 'schedule' },
    { id: DashboardRequestStatus.ToClose, name: RequestStatusMapper.getDashboardStatusDisplayName(DashboardRequestStatus.ToClose), count: 0, color: '#42a5f5', icon: 'folder' },
    { id: DashboardRequestStatus.Unpaid, name: RequestStatusMapper.getDashboardStatusDisplayName(DashboardRequestStatus.Unpaid), count: 0, color: '#ab47bc', icon: 'attach_money' }
  ];

  // Current state
  selectedState = DashboardRequestStatus.New;
  
  // Table data
  displayedColumns: string[] = [];
  dataSource: AdminRequestData[] = [];
  totalCount = 0;
  totalPages = 0;
  currentPage = 1;
  pageSize = 10;
  
  // Pagination request
  paginationRequest: PaginationRequest<PaginationDashboardFilters> = {
    pageIndex: 1,
    pageSize: 10,
    sortColumn: '',
    sortDirection: 'asc',
    searchString: '',
    filters: {
      searchTerm: '',
      requestType: undefined,
      regionId: undefined,
      fromDate: undefined,
      toDate: undefined
    }
  };

  // Loading states
  isLoading = false;
  isLoadingStates = false;

  // Request types for filter
  requestTypes = [
    { value: null, label: 'All' },
    { value: RequestType.Patient, label: 'Patient' },
    { value: RequestType.Family, label: 'Family/Friend' },
    { value: RequestType.Business, label: 'Business' },
    { value: RequestType.Concierge, label: 'Concierge' }
  ];

  constructor(
    private authService: AuthService,
    private adminDashboardService: AdminDashboardService
  ) {}

  ngOnInit() {
    this.loadStateCounts();
    this.loadRequests();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadStateCounts() {
    this.isLoadingStates = true;
    this.adminDashboardService.getStateCounts()
      .subscribe({
        next: (states) => {
          this.dashboardStates = states;
          this.isLoadingStates = false;
        },
        error: (error) => {
          console.error('Error loading state counts:', error);
          this.isLoadingStates = false;
        }
      });
  }

  loadRequests() {
    this.isLoading = true;
    this.paginationRequest.pageIndex = this.currentPage;
    this.paginationRequest.pageSize = this.pageSize;
    
    this.adminDashboardService.getRequestsByState(this.selectedState, this.paginationRequest)
      .subscribe({
        next: (response: PaginationResponse<AdminRequestData>) => {
          this.dataSource = response.items;
          this.totalCount = response.totalCount;
          this.totalPages = response.totalPages;
          this.currentPage = response.pageIndex;
          this.pageSize = response.pageSize;
          this.updateDisplayedColumns();
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading requests:', error);
          this.isLoading = false;
        }
      });
  }

  onStateChange(state: DashboardRequestStatus) {
    this.selectedState = state;
    this.currentPage = 1;
    this.paginationRequest.pageIndex = 1;
    this.loadRequests();
  }

  onSearchChange(searchTerm: string) {
    this.paginationRequest.searchString = searchTerm;
    this.paginationRequest.filters.searchTerm = searchTerm;
    this.currentPage = 1;
    this.paginationRequest.pageIndex = 1;
    this.loadRequests();
  }

  onRequestTypeChange(requestType: string) {
    this.paginationRequest.filters.requestType = requestType ? parseInt(requestType) : undefined;
    this.currentPage = 1;
    this.paginationRequest.pageIndex = 1;
    this.loadRequests();
  }

  onPageChange(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex + 1;
    this.paginationRequest.pageIndex = event.pageIndex + 1;
    this.paginationRequest.pageSize = event.pageSize;
    this.loadRequests();
  }

  private updateDisplayedColumns() {
    // Simplified column logic, now based on selectedState
    this.displayedColumns = [
      'patientFullName',
      'dateOfBirth', 
      'requestorName',
      'requestedDate',
      'phone',
      'address',
      'actions'
    ];

    // Add physician and date of service columns for states that have them
    if (this.selectedState !== DashboardRequestStatus.New) {
      this.displayedColumns.splice(3, 0, 'physicianName', 'dateOfService');
    }

    // Add request status column for To-Close state
    if (this.selectedState === DashboardRequestStatus.ToClose) {
      this.displayedColumns.splice(-1, 0, 'requestStatus');
    }
  }

  getRequestActions(request: AdminRequestData): RequestAction[] {
    switch (this.selectedState) {
      case DashboardRequestStatus.New:
        return [
          { label: 'Assign Request', action: 'assign', icon: 'person_add' },
          { label: 'Cancel Request', action: 'cancel', icon: 'cancel' },
          { label: 'View Request', action: 'view', icon: 'visibility' },
          { label: 'View Documents', action: 'documents', icon: 'description' }
        ];

      case DashboardRequestStatus.Pending:
        return [
          { label: 'View Request', action: 'view', icon: 'visibility' },
          { label: 'View Documents', action: 'documents', icon: 'description' },
          { label: 'View Notes', action: 'notes', icon: 'note' },
          { label: 'Send Agreement', action: 'send-agreement', icon: 'send' },
          { label: 'Clear Case', action: 'clear', icon: 'check_circle' }
        ];

      case DashboardRequestStatus.Active:
      case DashboardRequestStatus.Conclude:
        return [
          { label: 'Send Order', action: 'send-order', icon: 'shopping_cart' },
          { label: 'View Request', action: 'view', icon: 'visibility' },
          { label: 'View Documents', action: 'documents', icon: 'description' },
          { label: 'View Notes', action: 'notes', icon: 'note' }
        ];

      case DashboardRequestStatus.ToClose:
        return [
          { label: 'View Request', action: 'view', icon: 'visibility' },
          { label: 'View Documents', action: 'documents', icon: 'description' },
          { label: 'View Notes', action: 'notes', icon: 'note' },
          { label: 'Close Case', action: 'close', icon: 'close' }
        ];

      case DashboardRequestStatus.Unpaid:
        return [
          { label: 'View Request', action: 'view', icon: 'visibility' },
          { label: 'View Documents', action: 'documents', icon: 'description' },
          { label: 'View Notes', action: 'notes', icon: 'note' }
        ];

      default:
        return [];
    }
  }

  onActionClick(action: string, request: AdminRequestData) {
    console.log(`Action ${action} clicked for request ${request.id}`);
    // TODO: Implement action handlers
  }

  exportRequests() {
    this.adminDashboardService.exportRequests(this.selectedState, this.paginationRequest).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `requests_${RequestStatusMapper.getDashboardStatusDisplayName(this.selectedState).toLowerCase()}_${new Date().toISOString().split('T')[0]}.csv`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Error exporting requests:', error);
      }
    });
  }

  exportAllRequests() {
    this.adminDashboardService.exportAllRequests(this.paginationRequest).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `all_requests_${new Date().toISOString().split('T')[0]}.csv`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Error exporting all requests:', error);
      }
    });
  }

  logout() {
    this.authService.logout();
  }

  getRequestTypeClass(requestType: number): string {
    return RequestStatusMapper.getRequestTypeClass(requestType);
  }
}
