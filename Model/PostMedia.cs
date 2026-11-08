using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netgram.Models
{
    public class PostMedia
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string MediaUrl { get; set; } = string.Empty;
        [Required]
        public string CloudinaryPublicId { get; set; } = string.Empty;

        [Required]
        public string MediaType { get; set; }=string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int PostId { get; set; }

        [ForeignKey(nameof(PostId))]
        public Post? Post { get; set; }

    }
}