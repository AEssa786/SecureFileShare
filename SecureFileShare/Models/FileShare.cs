using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureFileShare.Models
{
    public class FileShare
    {

        [Key]
        public int ShareId { get; set; }
        [Required]
        public int SharedFileId { get; set; }
        [Required]
        public string SharedWithId { get; set; }
        public DateTime ShareDate { get; set; } = DateTime.UtcNow;
        [Required]
        public string PermissionType{ get; set; }
        public DateTime? ExpiryDate{ get; set; }
        [ForeignKey("SharedFileId")]
        public File SharedFile { get; set; }
        [ForeignKey("SharedWithId")]
        public ApplicationUser SharedWith { get; set; }

    }
}
