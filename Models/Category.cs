using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace netgram.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}