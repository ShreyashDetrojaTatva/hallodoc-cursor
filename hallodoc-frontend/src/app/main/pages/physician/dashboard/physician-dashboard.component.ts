import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
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
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { MatMenuTrigger } from '@angular/material/menu';
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
import { PhysicianDashboardService } from '@main/services/physician-dashboard.service';
import { AdminRequestService } from '@main/services/admin-request.service';
import { AcceptRequestComponent } from '@main/components/physician/accept-request/accept-request.component';
import { AcceptRequestData } from '@main/interfaces/physician/accept-request.interface';

@Component({
  selector: 'app-physician-dashboard',
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
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatDialogModule
  ],
  templateUrl: './physician-dashboard.component.html',
  styleUrl: './physician-dashboard.component.scss'
})
export class PhysicianDashboardComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  @ViewChild(MatMenuTrigger) menuTrigger!: MatMenuTrigger;

  // Make enum available in template
  DashboardRequestStatus = DashboardRequestStatus;

  // Dashboard states (only for physicians)
  dashboardStates: DashboardState[] = [
    { id: DashboardRequestStatus.New, name: RequestStatusMapper.getDashboardStatusDisplayName(DashboardRequestStatus.New), count: 0, color: '#1976d2', icon: 'settings' },
    { id: DashboardRequestStatus.Pending, name: RequestStatusMapper.getDashboardStatusDisplayName(DashboardRequestStatus.Pending), count: 0, color: '#42a5f5', icon: 'notifications' },
    { id: DashboardRequestStatus.Active, name: RequestStatusMapper.getDashboardStatusDisplayName(DashboardRequestStatus.Active), count: 0, color: '#66bb6a', icon: 'check_circle' },
    { id: DashboardRequestStatus.Conclude, name: RequestStatusMapper.getDashboardStatusDisplayName(DashboardRequestStatus.Conclude), count: 0, color: '#ec407a', icon: 'schedule' }
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

  // Current request for menu actions
  currentRequest: AdminRequestData | null = null;

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
    private physicianDashboardService: PhysicianDashboardService,
    private adminRequestService: AdminRequestService,
    private router: Router,
    private dialog: MatDialog
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
    this.physicianDashboardService.getStateCounts()
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
    this.updateDisplayedColumns();

    this.physicianDashboardService.getRequestsByState(this.selectedState, this.paginationRequest)
      .subscribe({
        next: (response: PaginationResponse<AdminRequestData>) => {
          this.dataSource = response.items;
          this.totalCount = response.totalCount;
          this.totalPages = response.totalPages;
          this.currentPage = response.pageIndex;
          this.pageSize = response.pageSize;
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
    this.paginationRequest.pageIndex = 1; // Reset to first page
    this.loadRequests();
  }

  onSearchChange(searchTerm: string) {
    this.paginationRequest.searchString = searchTerm;
    this.paginationRequest.pageIndex = 1; // Reset to first page
    this.loadRequests();
  }

  onRequestTypeChange(requestType: string) {
    this.paginationRequest.filters.requestType = requestType ? parseInt(requestType) : undefined;
    this.paginationRequest.pageIndex = 1; // Reset to first page
    this.loadRequests();
  }

  onPageChange(event: PageEvent) {
    this.paginationRequest.pageIndex = event.pageIndex + 1;
    this.paginationRequest.pageSize = event.pageSize;
    this.loadRequests();
  }

  private updateDisplayedColumns() {
    this.displayedColumns = [
      'patientFullName',
      'dateOfBirth', 
      'requestorName',
      'requestedDate',
      'phone',
      'address',
      'actions'
    ];
    
    // Add physician name column for all states
    this.displayedColumns.splice(3, 0, 'physicianName');
    
    // Add date of service for non-NEW states
    if (this.selectedState !== DashboardRequestStatus.New) {
      this.displayedColumns.splice(4, 0, 'dateOfService');
    }
  }

  getRequestActions(request: AdminRequestData): RequestAction[] {
    const actions: RequestAction[] = [];

    // Always available actions
    actions.push({ label: 'View Request', action: 'view', icon: 'visibility' });
    actions.push({ label: 'View Documents', action: 'documents', icon: 'description' });

    // State-specific actions
    switch (this.selectedState) {
      case DashboardRequestStatus.New:
        actions.push({ label: 'Accept Request', action: 'accept', icon: 'check_circle' });
        break;
      
      case DashboardRequestStatus.Pending:
        actions.push({ label: 'Send Agreement', action: 'send-agreement', icon: 'send' });
        break;
      
      case DashboardRequestStatus.Active:
        actions.push({ label: 'Accept Request', action: 'accept', icon: 'check_circle' });
        actions.push({ label: 'Create/Update Encounter Form', action: 'encounter-form', icon: 'edit_note' });
        break;
      
      case DashboardRequestStatus.Conclude:
        actions.push({ label: 'Accept Request', action: 'accept', icon: 'check_circle' });
        actions.push({ label: 'Conclude Care', action: 'conclude-care', icon: 'check_circle_outline' });
        break;
    }

    return actions;
  }

  setCurrentRequest(request: AdminRequestData) {
    this.currentRequest = request;
  }

  onMenuButtonClick(request: AdminRequestData) {
    this.setCurrentRequest(request);
  }

  onMenuItemMouseDown(action: string) {
    // Prevent menu from closing immediately
    event?.preventDefault();
  }

  onActionClick(action: string, request: AdminRequestData) {
    switch (action) {
      case 'view':
        // Navigate to view request page
        this.router.navigate(['/physician/request', request.id, 'view']);
        break;
        
      case 'documents':
        // Navigate to documents page (to be implemented)
        console.log('Navigate to documents page for request:', request.id);
        break;
        
      case 'accept':
        this.openAcceptDialog(request);
        break;
        
      case 'send-agreement':
        // Send agreement (to be implemented)
        console.log('Send agreement for request:', request.id);
        break;
        
      case 'encounter-form':
        // Create/Update encounter form (to be implemented)
        console.log('Create/Update encounter form for request:', request.id);
        break;
        
      case 'conclude-care':
        // Conclude care (to be implemented)
        console.log('Conclude care for request:', request.id);
        break;
        
      default:
        console.log('Unknown action:', action, 'for request:', request.id);
        break;
    }
  }

  openAcceptDialog(request: AdminRequestData) {
    const dialogRef = this.dialog.open(AcceptRequestComponent, {
      width: '500px',
      maxWidth: '90vw',
      maxHeight: '90vh',
      disableClose: false,
      autoFocus: true,
      data: {
        requestId: request.id,
        patientName: request.patientFullName
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result?.success) {
        const acceptRequestData: AcceptRequestData = {
          requestId: result.requestId,
          notes: result.notes
        };

        this.physicianDashboardService.acceptRequest(acceptRequestData).subscribe({
          next: (response) => {
            console.log('Request accepted successfully:', response);
            // Refresh the data
            this.loadRequests();
            this.loadStateCounts();
          },
          error: (error) => {
            console.error('Error accepting request:', error);
          }
        });
      }
    });
  }

  exportRequests() {
    this.physicianDashboardService.exportRequests(this.selectedState, this.paginationRequest).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `physician-requests-${this.selectedState}-${new Date().toISOString().split('T')[0]}.csv`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Error exporting requests:', error);
      }
    });
  }

  exportAllRequests() {
    this.physicianDashboardService.exportAllRequests(this.paginationRequest).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `physician-all-requests-${new Date().toISOString().split('T')[0]}.csv`;
        link.click();
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

  getMenuReference(requestId: number): string {
    return `menu-${requestId}`;
  }

  getRequestStatusDisplayName(statusValue: string | null): string {
    if (!statusValue) return '-';
    
    const numericStatus = parseInt(statusValue, 10);
    if (isNaN(numericStatus)) return statusValue;
    
    return RequestStatusMapper.getRequestStatusDisplayName(numericStatus);
  }
} 