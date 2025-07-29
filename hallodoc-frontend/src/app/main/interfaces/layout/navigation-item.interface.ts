import { AccountType } from '../../enums';

export interface NavigationItem {
  label: string;
  route: string;
  icon: string;
  roles?: string[];
  accountTypes?: AccountType[];
} 