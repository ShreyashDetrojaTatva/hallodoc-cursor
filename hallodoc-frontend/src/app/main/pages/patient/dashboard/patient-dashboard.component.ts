import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../services/auth/auth.service';
import { RequestService } from '../../../services/request/request.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-patient-dashboard',
  templateUrl: './patient-dashboard.component.html',
  styleUrls: ['./patient-dashboard.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatSortModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ]
})
export class PatientDashboardComponent implements OnInit {
  displayedColumns: string[] = ['requestId', 'requestType', 'status', 'createdAt', 'firstName', 'lastName', 'symptoms', 'actions'];
  dataSource = new MatTableDataSource<any>();
  isLoading = false;
  error: string | null = null;

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private authService: AuthService,
    private requestService: RequestService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadRequests();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  loadRequests() {
    this.isLoading = true;
    this.error = null;

    this.requestService.getPatientRequests()
      .subscribe({
        next: (data) => {
          this.dataSource.data = data;
          this.isLoading = false;
        },
        error: (error) => {
          this.error = error.message || 'Failed to load requests. Please try again.';
          this.isLoading = false;
          if (error.message.includes('Please log in')) {
            this.authService.logout(); // This will redirect to login
          }
        }
      });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  viewDocuments(requestId: number) {
    this.router.navigate(['/patient/requests', requestId, 'documents']);
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

  refresh() {
    this.loadRequests();
  }
} 