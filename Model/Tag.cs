using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace netgram.Models
{
    public class Tag
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}