using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureFileShare.Models
{
    public class File
    {

        [Key]
        public int FileId { get; set; }
        public long Size { get; set; }
        [Required]
        public string OwnerId{ get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public bool IsShared { get; set; } = false;
        [ForeignKey("OwnerId")]
        public ApplicationUser Owner { get; set; }

    }
}
