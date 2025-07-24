import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { PatientRequestFormComponent } from './patient/patient-request-form.component';
import { FamilyRequestFormComponent } from './family/family-request-form.component';
import { ConciergeRequestFormComponent } from './concierge/concierge-request-form.component';
import { BusinessRequestFormComponent } from './business/business-request-form.component';
import { HeaderComponent } from '../../../core/components/shared/header.component';
import { FooterComponent } from '../../../core/components/shared/footer.component';

@Component({
  selector: 'app-request-form-placeholder',
  standalone: true,
  imports: [
    CommonModule, MatCardModule,
    PatientRequestFormComponent,
    FamilyRequestFormComponent,
    ConciergeRequestFormComponent,
    BusinessRequestFormComponent,
    HeaderComponent, FooterComponent
  ],
  templateUrl: './request-form-placeholder.component.html',
  styleUrls: ['./request-form-placeholder.component.scss']
})
export class RequestFormPlaceholderComponent {
  type = '';
  public currentYear = new Date().getFullYear();
  constructor(private route: ActivatedRoute) {
    this.route.params.subscribe(params => {
      this.type = params['type'];
    });
  }
} 