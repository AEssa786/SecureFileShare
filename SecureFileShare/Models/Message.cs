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
        public DateTime Timestamp { get; set; } = DateTime.Now;
        [ForeignKey("SenderId")]
        public virtual ApplicationUser Sender { get; set; }
        [ForeignKey("RecipientId")]
        public virtual ApplicationUser Recipient { get; set; }
        // TODO: Add Read Status to messages to persist the unread notification badge on the chat page until the user reads the message
        // TODO: Add visual element to read status on the chat page, such as a checkmark or different background color for read vs unread messages
    }
}
