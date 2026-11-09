using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netgram.Data;
using netgram.Models;

namespace netgram.Controllers
{
    [Authorize]
    public class LikesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LikesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(int id)
        {
            var postIsPublished = await _context.Posts.AnyAsync(post =>
                post.Id == id && post.Status == "Published");
            if (!postIsPublished)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge();
            }

            var alreadyLiked = await _context.PostLikes.AnyAsync(like =>
                like.PostId == id && like.UserId == userId);
            if (!alreadyLiked)
            {
                _context.PostLikes.Add(new PostLike
                {
                    PostId = id,
                    UserId = userId
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Posts", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlike(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge();
            }

            var like = await _context.PostLikes.FirstOrDefaultAsync(existingLike =>
                existingLike.PostId == id && existingLike.UserId == userId);
            if (like != null)
            {
                _context.PostLikes.Remove(like);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Posts", new { id });
        }
    }
}
