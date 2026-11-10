namespace WebShop.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public Role Role { get; set; }
        public int RoleId { get; set; }

        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockoutUntil { get; set; } = null;

    }
}
