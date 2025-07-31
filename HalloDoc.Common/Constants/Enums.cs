namespace HalloDoc.Common.Constants
{
    public enum AccountType
    {
        Admin = 1,
        Physician = 2,
        Patient = 3
    }

    public enum Status
    {
        Active = 1,
        Inactive = 2
    }

    public enum RequestType
    {
        Patient = 1,
        Family = 2,
        Concierge = 3,
        Business = 4
    }

    public enum RequestorType
    {
        Patient = 1,
        Family = 2,
        Concierge = 3,
        Business = 4
    }

    // Actual Request Statuses (Database States)
    public enum RequestStatus
    {
        Unassigned = 1,
        Accepted = 2,
        Cancelled = 3,
        MDEnRoute = 4,
        MDONSite = 5,
        Conclude = 6,
        CancelledByPatient = 7,
        Closed = 8,
        Unpaid = 9,
        Clear = 10,
        Blocked = 11
    }

    // Dashboard Request Status Enums (for admin dashboard filtering)
    public enum DashboardRequestStatus
    {
        New = 1,
        Pending = 2,
        Active = 3,
        Conclude = 4,
        ToClose = 5,
        Unpaid = 6
    }
} 