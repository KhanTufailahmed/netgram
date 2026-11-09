using netgram.Models;

namespace netgram.ViewModel
{
    public class PostDetailsViewModel
    {
        public Post Post { get; set; } = null!;

        public bool UserHasLiked { get; set; }

        public bool IsAuthor { get; set; }
    }
}
