import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { SampleButtonComponent } from '@core/components';
import { AuthService } from '@main/services';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';
import { ProfileData } from '@main/interfaces';
import { PingData } from '@main/interfaces';
import { MatCardModule } from "@angular/material/card";

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, SampleButtonComponent, MatCardModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  pingMessage = '';
  displayedColumns: string[] = ['id', 'message'];
  dataSource: PingData[] = [];
  user: ProfileData | null = null;

  constructor(private http: HttpClient, private authService: AuthService) {}

  ngOnInit() {
    this.user = this.authService.getCurrentUser();
  }

  logout() {
    this.authService.logout();
  }

  pingApi() {
    this.http.get<string>('/api/ping/first').subscribe(data => {
      this.pingMessage = data;
    });
  }

  loadAllPings() {
    this.http.get<PingData[]>('/api/ping/all').subscribe(data => {
      this.dataSource = data;
    });
  }
}
