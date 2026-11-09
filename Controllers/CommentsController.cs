using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netgram.Data;
using netgram.Models;
using netgram.ViewModel;

namespace netgram.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCommentViewModel model)
        {
            var postIsPublished = await _context.Posts.AnyAsync(post =>
                post.Id == model.PostId && post.Status == "Published");
            if (!postIsPublished)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                TempData["CommentError"] = "Please enter a comment with 1 to 1000 characters.";
                return RedirectToAction("Details", "Posts", new { id = model.PostId });
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge();
            }

            _context.Comments.Add(new Comment
            {
                PostId = model.PostId,
                UserId = userId,
                Content = model.Content
            });
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Posts", new { id = model.PostId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var comment = await _context.Comments
                .FirstOrDefaultAsync(existingComment =>
                    existingComment.Id == id && existingComment.UserId == userId);

            if (comment == null)
            {
                return NotFound();
            }

            var postId = comment.PostId;
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Posts", new { id = postId });
        }
    }
}
