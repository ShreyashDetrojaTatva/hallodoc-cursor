using HalloDoc.Common.Constants;

namespace HalloDoc.Common.Utility
{
    public static class RequestStatusMapper
    {
        public static int[] GetStatusIdsForState(DashboardRequestStatus state)
        {
            return state switch
            {
                DashboardRequestStatus.New => new[] { (int)RequestStatus.Unassigned }, // Unassigned
                DashboardRequestStatus.Pending => new[] { (int)RequestStatus.Accepted }, // Accepted
                DashboardRequestStatus.Active => new[] { (int)RequestStatus.MDEnRoute, (int)RequestStatus.MDONSite }, // MDEnRoute, MDONSite
                DashboardRequestStatus.Conclude => new[] { (int)RequestStatus.Conclude }, // Conclude
                DashboardRequestStatus.ToClose => new[] { (int)RequestStatus.Closed, (int)RequestStatus.Cancelled, (int)RequestStatus.CancelledByPatient }, // Closed, Cancelled, CancelledByPatient
                DashboardRequestStatus.Unpaid => new[] { (int)RequestStatus.Unpaid }, // Unpaid
                _ => new int[0]
            };
        }
    }
} 