import { RequestStatus, DashboardRequestStatus } from '@main/enums';

/**
 * Maps RequestStatus to DashboardRequestStatus based on the admin plan
 */
export class RequestStatusMapper {
  
  /**
   * Maps RequestStatus to DashboardRequestStatus
   */
  static toDashboardStatus(requestStatus: RequestStatus): DashboardRequestStatus {
    switch (requestStatus) {
      case RequestStatus.Unassigned:
        return DashboardRequestStatus.New;
      
      case RequestStatus.Accepted:
        return DashboardRequestStatus.Pending;
      
      case RequestStatus.MDEnRoute:
      case RequestStatus.MDONSite:
        return DashboardRequestStatus.Active;
      
      case RequestStatus.Conclude:
        return DashboardRequestStatus.Conclude;
      
      case RequestStatus.Closed:
      case RequestStatus.Cancelled:
      case RequestStatus.CancelledByPatient:
        return DashboardRequestStatus.ToClose;
      
      case RequestStatus.Unpaid:
        return DashboardRequestStatus.Unpaid;
      
      default:
        return DashboardRequestStatus.New;
    }
  }

  /**
   * Gets all RequestStatus values that map to a specific DashboardRequestStatus
   */
  static getRequestStatusesForDashboardStatus(dashboardStatus: DashboardRequestStatus): RequestStatus[] {
    switch (dashboardStatus) {
      case DashboardRequestStatus.New:
        return [RequestStatus.Unassigned];
      
      case DashboardRequestStatus.Pending:
        return [RequestStatus.Accepted];
      
      case DashboardRequestStatus.Active:
        return [RequestStatus.MDEnRoute, RequestStatus.MDONSite];
      
      case DashboardRequestStatus.Conclude:
        return [RequestStatus.Conclude];
      
      case DashboardRequestStatus.ToClose:
        return [RequestStatus.Closed, RequestStatus.Cancelled, RequestStatus.CancelledByPatient];
      
      case DashboardRequestStatus.Unpaid:
        return [RequestStatus.Unpaid];
      
      default:
        return [];
    }
  }

  /**
   * Gets the display name for a RequestStatus
   */
  static getRequestStatusDisplayName(status: RequestStatus): string {
    switch (status) {
      case RequestStatus.Unassigned:
        return 'Unassigned';
      case RequestStatus.Accepted:
        return 'Accepted';
      case RequestStatus.Cancelled:
        return 'Cancelled';
      case RequestStatus.MDEnRoute:
        return 'MD En Route';
      case RequestStatus.MDONSite:
        return 'MD On Site';
      case RequestStatus.Conclude:
        return 'Conclude';
      case RequestStatus.CancelledByPatient:
        return 'Cancelled by Patient';
      case RequestStatus.Closed:
        return 'Closed';
      case RequestStatus.Unpaid:
        return 'Unpaid';
      case RequestStatus.Clear:
        return 'Clear';
      case RequestStatus.Blocked:
        return 'Blocked';
      default:
        return 'Unknown';
    }
  }

  /**
   * Gets the display name for a DashboardRequestStatus
   */
  static getDashboardStatusDisplayName(status: DashboardRequestStatus): string {
    switch (status) {
      case DashboardRequestStatus.New:
        return 'NEW';
      case DashboardRequestStatus.Pending:
        return 'PENDING';
      case DashboardRequestStatus.Active:
        return 'ACTIVE';
      case DashboardRequestStatus.Conclude:
        return 'CONCLUDE';
      case DashboardRequestStatus.ToClose:
        return 'TO CLOSE';
      case DashboardRequestStatus.Unpaid:
        return 'UNPAID';
      default:
        return 'UNKNOWN';
    }
  }
} 