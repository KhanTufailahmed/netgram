using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using netgram.Models;

namespace netgram.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Draft";
        //foreign key refereing to User
        [Required]
        public string UserId { get; set; } = String.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        //foreign key refering to Category
        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<PostTag> PostTags { get; set; }

        public ICollection<PostMedia> Media { get; set; }

    }
}