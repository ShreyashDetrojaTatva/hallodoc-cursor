import { RequestType, RequestorType } from '@main/enums';

export interface RequestData {
  requestType: RequestType;
  requestorType: RequestorType;
  requestorFirstName?: string;
  requestorLastName?: string;
  requestorEmail?: string;
  requestorPhone?: string;
  relationWithPatient?: string;
  hotelName?: string;
  propertyName?: string;
  caseNumber?: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  dob: Date | string;
  symptoms: string;
  street?: string;
  address?: string;
  city: string;
  state?: string;
  regionId?: number;
  zipCode: string;
  roomNo?: string;
  files?: File[];
} 