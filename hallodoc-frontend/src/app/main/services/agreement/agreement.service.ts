import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AGREEMENT_ENDPOINTS } from '@main/constants';
import {
  SendAgreementData,
  AgreementResponseData,
  AgreementDetailsData,
} from '@main/interfaces/agreement';

@Injectable({
  providedIn: 'root',
})
export class AgreementService {
  constructor(private http: HttpClient) {}

  sendAgreement(data: SendAgreementData): Observable<any> {
    return this.http.post(AGREEMENT_ENDPOINTS.SEND_AGREEMENT, data);
  }

  getAgreementDetails(token: string): Observable<AgreementDetailsData> {
    return this.http.get<AgreementDetailsData>(
      AGREEMENT_ENDPOINTS.GET_AGREEMENT_DETAILS(token)
    );
  }

  processAgreementResponse(data: AgreementResponseData): Observable<any> {
    return this.http.post(AGREEMENT_ENDPOINTS.PROCESS_AGREEMENT_RESPONSE, data);
  }

  validateToken(token: string): Observable<{ isValid: boolean }> {
    return this.http.get<{ isValid: boolean }>(
      AGREEMENT_ENDPOINTS.VALIDATE_TOKEN(token)
    );
  }
}
