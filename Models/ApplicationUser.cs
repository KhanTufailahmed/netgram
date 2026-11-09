using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace netgram.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string DisplayName { get; set; }

        public ICollection<Post> Posts { get; set; }=new List<Post>();

        public ICollection<Comment> Comments { get; set; }=new List<Comment>();

        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
    }
}