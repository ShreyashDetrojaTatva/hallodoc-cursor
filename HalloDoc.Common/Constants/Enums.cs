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

    public enum RequestStatus
    {
        New = 1,
        Pending = 2,
        Active = 3,
        Conclude = 4,
        ToClose = 5,
        Unpaid = 6
        // Add more as needed
    }

    public enum RequestorType
    {
        Patient = 1,
        Family = 2,
        Concierge = 3,
        Business = 4
    }
} 