export interface SendAgreementData {
  requestId: number;
  patientEmail: string;
}

export interface AgreementResponseData {
  token: string;
  isAccepted: boolean;
  cancellationReason?: string;
}

export interface AgreementDetailsData {
  requestId: number;
  patientName: string;
  patientEmail: string;
  createdAt: string;
  requestorName: string;
  requestorEmail: string;
  requestorPhone: string;
  symptoms?: string;
  isExpired: boolean;
  isUsed: boolean;
}

export interface AgreementTokenData {
  id: number;
  requestId: number;
  token: string;
  expiry: string;
  used: boolean;
  usedAt?: string;
  createdAt: string;
} 