using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureFileShare.Models
{
    public class Message
    {
        [Key]
        public int MessageId{ get; set; }
        [Required]
        public string SenderId{ get; set; }
        [Required]
        public string RecipientId{ get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [ForeignKey("SenderId")]
        public ApplicationUser Sender { get; set; }
        [ForeignKey("RecipientId")]
        public ApplicationUser Recipient { get; set; }

    }
}
