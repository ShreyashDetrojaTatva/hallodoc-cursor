import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { MatSelectModule } from '@angular/material/select';
import { ReactiveFormsModule, FormControl, FormGroup } from '@angular/forms';
import { RequestService } from '@main/services';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';
import { RequestDashboardData } from '@main/interfaces';
import { RequestStatus, RequestType } from '@main/enums';

@Component({
  selector: 'app-patient-dashboard',
  templateUrl: './patient-dashboard.component.html',
  styleUrls: ['./patient-dashboard.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    MatTableModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatChipsModule,
    MatSelectModule,
    ReactiveFormsModule
  ]
})
export class PatientDashboardComponent implements OnInit {
  @ViewChild(MatSort) sort!: MatSort;

  displayedColumns: string[] = ['requestId', 'requestType', 'status', 'createdAt', 'firstName', 'lastName', 'symptoms', 'actions'];
  dataSource = new MatTableDataSource<RequestDashboardData>();
  isLoading = false;
  error: string | null = null;
  
  // Filter controls
  filterForm = new FormGroup({
    searchTerm: new FormControl(''),
    requestType: new FormControl('')
  });

  // Filter options using enums
  requestTypeOptions = [
    { value: '', label: 'All Types' },
    { value: RequestType.Patient.toString(), label: 'Patient' },
    { value: RequestType.Family.toString(), label: 'Family/Friend' },
    { value: RequestType.Concierge.toString(), label: 'Concierge' },
    { value: RequestType.Business.toString(), label: 'Business' }
  ];

  constructor(
    private requestService: RequestService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadRequests();
    this.setupFilters();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
  }

  loadRequests() {
    this.isLoading = true;
    this.error = null;

    this.requestService.getPatientRequests()
      .pipe(
        catchError(err => {
          this.error = 'Failed to load requests. Please try again.';
          return of([]);
        }),
        finalize(() => this.isLoading = false)
      )
      .subscribe(requests => {
        this.dataSource.data = requests as RequestDashboardData[];
        this.applyFilters();
      });
  }

  setupFilters() {
    this.filterForm.valueChanges.subscribe(() => {
      this.applyFilters();
    });
  }

  applyFilters() {
    const searchTerm = this.filterForm.get('searchTerm')?.value?.toLowerCase() || '';
    const requestType = this.filterForm.get('requestType')?.value || '';

    this.dataSource.filterPredicate = (data: RequestDashboardData, filter: string) => {
      const filters = filter.split('|');
      const searchFilter = filters[0];
      const typeFilter = filters[1];

      // Search filter
      const matchesSearch = !searchFilter || 
        data.firstName?.toLowerCase().includes(searchFilter) ||
        data.lastName?.toLowerCase().includes(searchFilter) ||
        data.symptoms?.toLowerCase().includes(searchFilter) ||
        data.requestId?.toString().includes(searchFilter);

      // Request type filter
      const matchesType = !typeFilter || data.requestType === typeFilter;

      return matchesSearch && matchesType;
    };

    this.dataSource.filter = `${searchTerm}|${requestType}`;
  }

  clearFilters() {
    this.filterForm.reset();
  }

  viewDocuments(requestId: number) {
    console.log('Navigating to documents for request:', requestId);
    this.router.navigate(['/patient/requests', requestId, 'documents']);
  }

  refresh() {
    this.loadRequests();
  }

  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'pending':
        return '#FFA726'; // Orange
      case 'accepted':
        return '#66BB6A'; // Green
      case 'completed':
        return '#26C6DA'; // Blue
      case 'cancelled':
        return '#EF5350'; // Red
      default:
        return '#78909C'; // Grey
    }
  }

  getRequestTypeDisplayName(type: string): string {
    const typeOption = this.requestTypeOptions.find(option => option.value === type);
    return typeOption ? typeOption.label : type;
  }
} 