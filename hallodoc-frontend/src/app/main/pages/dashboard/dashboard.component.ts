import { Component } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { SampleButtonComponent } from '../../../core/components/shared/sample-button.component';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';

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
  dataSource: any[] = [];
  user: any = null;

  constructor(private http: HttpClient, private authService: AuthService, private router: Router) {}

  ngOnInit() {
    this.user = this.authService.getCurrentUser();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  pingApi() {
    this.http.get('/api/ping/first', { responseType: 'text' }).subscribe(result => {
      this.pingMessage = result;
    });
  }

  loadAllPings() {
    this.http.get<any[]>('/api/ping/all').subscribe(data => {
      this.dataSource = data;
    });
  }
}
