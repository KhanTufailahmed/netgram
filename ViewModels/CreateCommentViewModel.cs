using System.ComponentModel.DataAnnotations;

namespace netgram.ViewModel
{
    public class CreateCommentViewModel
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;
    }
}
