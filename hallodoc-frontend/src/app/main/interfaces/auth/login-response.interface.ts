import { ProfileData } from './profile-data.interface';

export interface LoginResponse {
  token: string;
  user: ProfileData;
} 