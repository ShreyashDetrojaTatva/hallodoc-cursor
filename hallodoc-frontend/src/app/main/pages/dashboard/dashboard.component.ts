import { Component } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { SampleButtonComponent } from '../../../core/components/shared/sample-button.component';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';
import { ProfileData } from '../../interfaces/auth/profile-data.interface';
import { PingData } from '../../interfaces/dashboard/ping-data.interface';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [HttpClientModule, MatButtonModule, MatCardModule, MatTableModule, SampleButtonComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  pingMessage = '';
  displayedColumns: string[] = ['id', 'message'];
  dataSource: PingData[] = [];
  user: ProfileData | null = null;

  constructor(private http: HttpClient, private authService: AuthService, private router: Router) {}

  ngOnInit() {
    this.user = this.authService.getCurrentUser();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
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
