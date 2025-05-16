using instademo.data;
using instademo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace instademo.Controllers
{
    public class videosController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly InstaReelDbContext context;
        public videosController(IWebHostEnvironment environment, InstaReelDbContext _context)
        {
            _environment = environment;
            context = _context;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Path to the uploads folder inside wwwroot
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var fineName = Path.GetFileName(file.FileName);
                var extension = Path.GetExtension(file.FileName).ToLower();
                var newFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                ViewBag.Message = "File uploaded successfully!";
                VideoClass obj = new VideoClass()
                {
                    Id = Guid.NewGuid(), 
                    VideoName = Path.GetFileName(file.FileName),
                    VideoPath = "~/uploads/"+newFileName
                };
                await context.Reels.AddAsync(obj);
                await context.SaveChangesAsync();
            }
            else
            {
                ViewBag.Message = "No file selected.";
            }

            return RedirectToAction("Index");
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Reels()
        {
            List<VideoClass> reels = await context.Reels.ToListAsync();
            return View(reels);
        }
    }
}
