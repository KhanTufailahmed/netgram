using System.ComponentModel.DataAnnotations;

namespace netgram.ViewModel
{
    public class CreatePostViewModel
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Status")]
        [RegularExpression("Draft|Published", ErrorMessage = "Choose Draft or Published.")]
        public string Status { get; set; } = "Draft";

        [Display(Name = "Tags")]
        public List<int> TagIds { get; set; } = new List<int>();
    }
}
