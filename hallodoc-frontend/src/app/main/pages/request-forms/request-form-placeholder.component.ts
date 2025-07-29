import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { PatientRequestFormComponent } from './patient/patient-request-form.component';
import { FamilyRequestFormComponent } from './family/family-request-form.component';
import { ConciergeRequestFormComponent } from './concierge/concierge-request-form.component';
import { BusinessRequestFormComponent } from './business/business-request-form.component';
import { HeaderComponent } from '@main/components';
import { FooterComponent } from '@core/components';

@Component({
  selector: 'app-request-form-placeholder',
  templateUrl: './request-form-placeholder.component.html',
  styleUrls: ['./request-form-placeholder.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    PatientRequestFormComponent,
    FamilyRequestFormComponent,
    ConciergeRequestFormComponent,
    BusinessRequestFormComponent,
    HeaderComponent,
    FooterComponent
  ]
})
export class RequestFormPlaceholderComponent implements OnInit {
  type = '';

  constructor(private route: ActivatedRoute) {}

  ngOnInit() {
    this.type = this.route.snapshot.params['type'];
  }
} 