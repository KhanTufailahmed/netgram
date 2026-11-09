using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netgram.Data;
using netgram.Models;
using netgram.ViewModel;

namespace netgram.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Where(post => post.Status == "Published")
                .Include(post => post.User)
                .Include(post => post.Category)
                .Include(post => post.PostTags)
                    .ThenInclude(postTag => postTag.Tag)
                .Include(post => post.Likes)
                .Include(post => post.Comments)
                .OrderByDescending(post => post.CreatedAt)
                .ToListAsync();

            return View(posts);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var post = await _context.Posts
                .Where(post => post.Id == id && post.Status == "Published")
                .Include(post => post.User)
                .Include(post => post.Category)
                .Include(post => post.PostTags)
                    .ThenInclude(postTag => postTag.Tag)
                .Include(post => post.Comments)
                    .ThenInclude(comment => comment.User)
                .Include(post => post.Likes)
                .FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var userHasLiked = userId != null &&
                await _context.PostLikes.AnyAsync(like =>
                    like.PostId == id && like.UserId == userId);

            var viewModel = new PostDetailsViewModel
            {
                Post = post,
                UserHasLiked = userHasLiked,
                IsAuthor = userId != null && post.UserId == userId
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadPostFormOptions();
            return View(new CreatePostViewModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadPostFormOptions();
                return View(model);
            }

            var categoryExists = await _context.Categories.AnyAsync(category =>
                category.Id == model.CategoryId);
            if (!categoryExists)
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Select a valid category.");
            }

            var tagIds = model.TagIds.Distinct().ToList();
            var validTagCount = await _context.Tags.CountAsync(tag => tagIds.Contains(tag.Id));
            if (validTagCount != tagIds.Count)
            {
                ModelState.AddModelError(nameof(model.TagIds), "One or more selected tags are invalid.");
            }

            if (!ModelState.IsValid)
            {
                await LoadPostFormOptions();
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge();
            }

            var post = new Post
            {
                Title = model.Title,
                Content = model.Content,
                CategoryId = model.CategoryId,
                Status = model.Status,
                UserId = userId
            };

            foreach (var tagId in tagIds)
            {
                post.PostTags.Add(new PostTag { TagId = tagId });
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            if (post.Status == "Draft")
            {
                return RedirectToAction(nameof(Edit), new { id = post.Id });
            }

            return RedirectToAction(nameof(Details), new { id = post.Id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var post = await _context.Posts
                .Include(post => post.PostTags)
                .FirstOrDefaultAsync(post => post.Id == id && post.UserId == userId);

            if (post == null)
            {
                return NotFound();
            }

            await LoadPostFormOptions();
            return View(new EditPostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CategoryId = post.CategoryId,
                Status = post.Status,
                TagIds = post.PostTags.Select(postTag => postTag.TagId).ToList()
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditPostViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var post = await _context.Posts
                .Include(existingPost => existingPost.PostTags)
                .FirstOrDefaultAsync(existingPost =>
                    existingPost.Id == id && existingPost.UserId == userId);

            if (post == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await LoadPostFormOptions();
                return View(model);
            }

            var categoryExists = await _context.Categories.AnyAsync(category =>
                category.Id == model.CategoryId);
            if (!categoryExists)
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Select a valid category.");
            }

            var tagIds = model.TagIds.Distinct().ToList();
            var validTagCount = await _context.Tags.CountAsync(tag => tagIds.Contains(tag.Id));
            if (validTagCount != tagIds.Count)
            {
                ModelState.AddModelError(nameof(model.TagIds), "One or more selected tags are invalid.");
            }

            if (!ModelState.IsValid)
            {
                await LoadPostFormOptions();
                return View(model);
            }

            post.Title = model.Title;
            post.Content = model.Content;
            post.CategoryId = model.CategoryId;
            post.Status = model.Status;
            post.UpdatedAt = DateTime.UtcNow;

            _context.PostTags.RemoveRange(post.PostTags);
            foreach (var tagId in tagIds)
            {
                _context.PostTags.Add(new PostTag { PostId = post.Id, TagId = tagId });
            }

            await _context.SaveChangesAsync();

            if (post.Status == "Draft")
            {
                return RedirectToAction(nameof(Edit), new { id = post.Id });
            }

            return RedirectToAction(nameof(Details), new { id = post.Id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var post = await _context.Posts
                .FirstOrDefaultAsync(existingPost =>
                    existingPost.Id == id && existingPost.UserId == userId);

            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadPostFormOptions()
        {
            ViewBag.Categories = await _context.Categories
                .OrderBy(category => category.Name)
                .ToListAsync();
            ViewBag.Tags = await _context.Tags
                .OrderBy(tag => tag.Name)
                .ToListAsync();
        }
    }
}
