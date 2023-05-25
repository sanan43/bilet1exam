using bilet1exam.Areas.Admin.ViewModel;
using bilet1exam.DAL;
using bilet1exam.Models;
using bilet1exam.Utilities.Constants;
using bilet1exam.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;


namespace bilet1exam.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PostsController : Controller
    {
        private readonly AppDbContext _context;
        public readonly IWebHostEnvironment _environment;

        public PostsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            List<Post> posts = await _context.Posts.Where(p => !p.IsDeleted).OrderByDescending(c => c.Id).ToListAsync();
            return View(posts);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostVM postVM)
        {
            if (!ModelState.IsValid)
            {
                return View(postVM);
            }
            if (!postVM.Photo.CheckContentType("image/"))
            {
                ModelState.AddModelError("Photo", ErrorMessages.FileMustBeImageType);
                return View(postVM);
            }
            if (!postVM.Photo.CheckFileSize(200))
            {
                ModelState.AddModelError("Photo", ErrorMessages.FileSizeMustLessThan200KB);
                return View(postVM);
            }

            string rootPath = Path.Combine(_environment.WebRootPath, "assets", "images");
            string fileName = await postVM.Photo.SaveAsync(rootPath);
            Post post = new Post()
            {
                Title = postVM.Title,
                Description = postVM.Description,
                ImagePath = fileName

            };
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }




        public async Task<IActionResult> Delete(int id)
        {
            Post post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();
            string filePath = Path.Combine(_environment.WebRootPath, "assets", "images", post.ImagePath);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int id)
        {
            Post post=await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();
            UpdatePostVM updatePostVM = new UpdatePostVM()
            {
                Description = post.Description,
                Id = id,
                Title = post.Title,
            };
            return View(updatePostVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdatePostVM postVM)
        {
            if (!ModelState.IsValid)
            {
                return View(postVM);
            }
            if (!postVM.Photo.CheckContentType("image/"))
            {
                ModelState.AddModelError("Photo", ErrorMessages.FileMustBeImageType);
                return View(postVM);
            }
            if (!postVM.Photo.CheckFileSize(200))
            {
                ModelState.AddModelError("Photo", ErrorMessages.FileSizeMustLessThan200KB);
                return View(postVM);
            }
            string rootPath = Path.Combine(_environment.WebRootPath, "assets", "images");
            Post post = await _context.Posts.FindAsync(postVM.Id);
            
            string filePath = Path.Combine(rootPath, post.ImagePath);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

           string newFileName= await postVM.Photo.SaveAsync(rootPath);
           
            post.ImagePath = newFileName;
            post.Title = postVM.Title;
            post.Description = postVM.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }



    }

}
