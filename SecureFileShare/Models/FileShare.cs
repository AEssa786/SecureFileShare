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
        [Required]
        public string SharedFromId{ get; set; }
        public DateTime ShareDate { get; set; } = DateTime.Now;
        [Required]
        public string PermissionType{ get; set; } = "Read"; // Default to read-only permission, can be extended to allow different permission types
        public DateTime? ExpiryDate{ get; set; } = null; // Optional expiry date for the share, can be null for no expiry
        [ForeignKey("SharedFileId")]
        public File SharedFile { get; set; }
        [ForeignKey("SharedWithId")]
        public ApplicationUser SharedWith { get; set; }
        [ForeignKey("SharedFromId")]
        public ApplicationUser SharedFrom { get; set; }

    }
}
