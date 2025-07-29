namespace HalloDoc.Services.Helpers
{
    public class ContextUser
    {
        public int UserId { get; set; }
        public int AccountType { get; set; }
        public int RoleId { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
} 