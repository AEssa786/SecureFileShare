using Microsoft.AspNetCore.Identity;

namespace SecureFileShare.Models
{
    public class ApplicationUser:IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool MfaEnabled { get; set; } = false;
        public string MfaSecretKey { get; set; } = string.Empty;
        public DateTime LastLoginDate { get; set; } = DateTime.UtcNow;
        public string LastLoginIp { get; set; } = string.Empty;
        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

    }
}
