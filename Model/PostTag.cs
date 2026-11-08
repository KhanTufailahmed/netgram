using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace netgram.Models
{
    public class PostTag
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        public int TagId { get; set; }
        [ForeignKey(nameof(PostId))]
        public Post? Post { get; set; }
        [ForeignKey(nameof(TagId))]
        public Tag? Tag { get; set; }
    }
}