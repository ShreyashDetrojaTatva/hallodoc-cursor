export interface RequestDetailsData {
  requestId: number;
  // Patient Information
  patientFirstName: string;
  patientLastName: string;
  patientDOB: string;
  patientPhone: string;
  patientEmail: string;
  patientStreet: string;
  patientCity: string;
  patientState: string;
  patientZipCode: string;
  
  // Request Information
  requestType: number;
  requestStatus: number;
  symptoms: string;
  createdAt: string;
  
  // Requestor Information
  requestorFirstName: string;
  requestorLastName: string;
  requestorPhone: string;
  requestorEmail: string;
  requestorRelation: string;
}

export interface UpdateRequestData {
  requestId: number;
  // Patient Information
  patientFirstName: string;
  patientLastName: string;
  patientDOB: Date | null;
  patientPhone: string;
  patientEmail: string;
  patientStreet: string;
  patientCity: string;
  patientState: string;
  patientZipCode: string;
  
  // Request Information
  symptoms: string;
  
  // Requestor Information (optional for patient requests)
  requestorFirstName?: string;
  requestorLastName?: string;
  requestorPhone?: string;
  requestorEmail?: string;
  requestorRelation?: string;
} 