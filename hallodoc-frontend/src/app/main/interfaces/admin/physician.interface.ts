export interface PhysicianData {
  physicianId: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  fullName: string;
}

export interface AssignRequestData {
  requestId: number;
  physicianId: number;
} 