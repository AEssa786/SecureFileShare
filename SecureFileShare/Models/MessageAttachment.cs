using System.ComponentModel.DataAnnotations.Schema;

namespace SecureFileShare.Models
{
    public class MessageAttachment
    {

        public int AttachmentId{ get; set; }
        public int MessageId { get; set; }
        public int FileId{ get; set; }
        [ForeignKey("MessageId")]
        public Message Message { get; set; }
        [ForeignKey("FileId")]
        public File File { get; set; }

    }
}
