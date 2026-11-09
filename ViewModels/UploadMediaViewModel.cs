using System.ComponentModel.DataAnnotations;

namespace netgram.Models
{
    public class UploadMediaViewModel
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        public IFormFile File { get; set; }

    }
}