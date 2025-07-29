import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { RequestService } from '../../../services/request/request.service';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';
import { RequestDashboardData } from '../../../interfaces/request/request-dashboard-data.interface';

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
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatChipsModule,
    ReactiveFormsModule
  ]
})
export class PatientDashboardComponent implements OnInit {
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  displayedColumns: string[] = ['requestId', 'requestType', 'status', 'createdAt', 'firstName', 'lastName', 'symptoms', 'actions'];
  dataSource = new MatTableDataSource<RequestDashboardData>();
  isLoading = false;
  error: string | null = null;
  searchControl = new FormControl('');

  constructor(
    private requestService: RequestService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadRequests();
    this.setupSearch();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
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
      });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  setupSearch() {
    this.searchControl.valueChanges.subscribe(value => {
      this.dataSource.filter = value || '';
    });
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
} 