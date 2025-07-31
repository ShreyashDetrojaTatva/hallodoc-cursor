import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
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
  DashboardFilters, 
  RequestAction 
} from '@main/interfaces';
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
  
  // Filters
  filters: DashboardFilters = {
    searchTerm: '',
    requestType: null,
    page: 1,
    pageSize: 10
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
    this.adminDashboardService.getStateCounts().subscribe({
      next: (stateCounts) => {
        this.dashboardStates = this.dashboardStates.map(state => {
          const countState = stateCounts.find(s => s.id === state.id);
          return { ...state, count: countState?.count || 0 };
        });
        this.isLoadingStates = false;
      },
      error: (err) => {
        console.error('Error loading state counts:', err);
        this.isLoadingStates = false;
      }
    });
  }

  loadRequests() {
    this.isLoading = true;
    this.adminDashboardService.getRequestsByState(this.selectedState, this.filters).subscribe({
      next: (response) => {
        this.dataSource = response.requests;
        this.totalCount = response.totalCount;
        this.updateDisplayedColumns();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading requests:', err);
        this.isLoading = false;
      }
    });
  }

  onStateChange(state: DashboardRequestStatus) {
    this.selectedState = state;
    this.filters.page = 1;
    this.loadRequests();
  }

  onSearchChange(searchTerm: string) {
    this.filters.searchTerm = searchTerm;
    this.filters.page = 1;
    this.loadRequests();
  }

  onRequestTypeChange(requestType: string) {
    this.filters.requestType = requestType ? parseInt(requestType) : null;
    this.filters.page = 1;
    this.loadRequests();
  }

  onPageChange(page: number) {
    this.filters.page = page;
    this.loadRequests();
  }

  private updateDisplayedColumns() {
    // Set columns based on selected state
    switch (this.selectedState) {
      case DashboardRequestStatus.New:
        this.displayedColumns = ['patientFullName', 'dateOfBirth', 'requestorName', 'requestedDate', 'phone', 'address', 'actions'];
        break;
      case DashboardRequestStatus.Pending:
      case DashboardRequestStatus.Active:
      case DashboardRequestStatus.Conclude:
        this.displayedColumns = ['patientFullName', 'dateOfBirth', 'requestorName', 'physicianName', 'dateOfService', 'phone', 'address', 'actions'];
        break;
      case DashboardRequestStatus.ToClose:
      case DashboardRequestStatus.Unpaid:
        this.displayedColumns = ['patientFullName', 'dateOfBirth', 'requestorName', 'physicianName', 'dateOfService', 'phone', 'address', 'requestStatus', 'actions'];
        break;
      default:
        this.displayedColumns = ['patientFullName', 'dateOfBirth', 'requestorName', 'requestedDate', 'phone', 'address', 'actions'];
    }
  }

  getRequestActions(request: AdminRequestData): RequestAction[] {
    const actions: RequestAction[] = [];

    // Add state-specific actions based on selectedState
    switch (this.selectedState) {
      case DashboardRequestStatus.New:
        actions.push(
          { id: 'assign', label: 'Assign Request', icon: 'assignment', action: 'assign' },
          { id: 'cancel', label: 'Cancel Request', icon: 'cancel', action: 'cancel' },
          { id: 'view', label: 'View Request', icon: 'visibility', action: 'view' },
          { id: 'documents', label: 'View Documents', icon: 'folder', action: 'documents' }
        );
        break;

      case DashboardRequestStatus.Pending:
        actions.push(
          { id: 'view', label: 'View Request', icon: 'visibility', action: 'view' },
          { id: 'documents', label: 'View Documents', icon: 'folder', action: 'documents' },
          { id: 'notes', label: 'View Notes', icon: 'note', action: 'notes' },
          { id: 'agreement', label: 'Send Agreement', icon: 'send', action: 'agreement' },
          { id: 'clear', label: 'Clear Case', icon: 'clear', action: 'clear' }
        );
        break;

      case DashboardRequestStatus.Active:
      case DashboardRequestStatus.Conclude:
        actions.push(
          { id: 'order', label: 'Send Order', icon: 'shopping_cart', action: 'order' },
          { id: 'view', label: 'View Request', icon: 'visibility', action: 'view' },
          { id: 'documents', label: 'View Documents', icon: 'folder', action: 'documents' },
          { id: 'notes', label: 'View Notes', icon: 'note', action: 'notes' }
        );
        break;

      case DashboardRequestStatus.ToClose:
        actions.push(
          { id: 'view', label: 'View Request', icon: 'visibility', action: 'view' },
          { id: 'documents', label: 'View Documents', icon: 'folder', action: 'documents' },
          { id: 'notes', label: 'View Notes', icon: 'note', action: 'notes' },
          { id: 'close', label: 'Close Case', icon: 'close', action: 'close' }
        );
        break;

      case DashboardRequestStatus.Unpaid:
        actions.push(
          { id: 'view', label: 'View Request', icon: 'visibility', action: 'view' },
          { id: 'documents', label: 'View Documents', icon: 'folder', action: 'documents' },
          { id: 'notes', label: 'View Notes', icon: 'note', action: 'notes' }
        );
        break;
    }

    return actions;
  }

  onActionClick(action: string, request: AdminRequestData) {
    console.log(`Action ${action} clicked for request ${request.id}`);
    // Implement action logic here
  }

  exportRequests() {
    this.adminDashboardService.exportRequests(this.selectedState, this.filters).subscribe({
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
      error: (err) => {
        console.error('Error exporting requests:', err);
      }
    });
  }

  exportAllRequests() {
    this.adminDashboardService.exportAllRequests(this.filters).subscribe({
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
      error: (err) => {
        console.error('Error exporting all requests:', err);
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
