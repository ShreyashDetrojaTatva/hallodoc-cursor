export interface ProfileData {
  firstName: string;
  lastName: string;
  email: string;
  username: string;
  phoneNumber: string;
  dob: Date | string;
  address: string;
  city: string;
  regionId: number;
  zipCode: string;
  accountType?: number;
  userId?: number;
  roleId?: number | null;
} 