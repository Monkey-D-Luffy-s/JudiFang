using instademo.data;
using instademo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Threading.Tasks;

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
                    VideoPath = newFileName,
                    Likes = 0,

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
            var rand = new Random();
            var index = rand.Next(0, reels.Count);
            VideoClass current = reels[index];

            var obj = await buildInitialModal(reels,current);
            return View(obj);
        }

        public async Task<IActionResult> Like(Guid id)
        {
            VideoClass current = await context.Reels.FirstOrDefaultAsync(x => x.Id == id);
            current.Likes = current.Likes + 1;
            await context.SaveChangesAsync();
            var obj = await buildInitialModal(null, current);
            return View("Reels", obj);
        }

        public async Task<InitialVideoModal> buildInitialModal(List<VideoClass>? reels, VideoClass current)
        {
            if(reels == null)
            {
                reels = await context.Reels.ToListAsync();
            }
            List<Comments> comments = await context.Comments.ToListAsync();
            List<Comments> reelComments = comments.Where(x => x.VideoId == current.Id).ToList();
            InitialVideoModal obj = new InitialVideoModal()
            {
                videos = reels,
                comments = comments,
                startVideo = current,
            };
            return obj;
        }

        [HttpPost]
        public async Task<IActionResult> SendComment( string msg, string Id)
        {
            DateTime currentTime = DateTime.Now;
            Comments comments = new Comments()
            {
                Id = Guid.NewGuid(),
                VideoId = Guid.Parse(Id),
                Name = "person1",
                Comment = msg,
                Date = currentTime.GetDateTimeFormats().First().ToString(),
            };
            await context.Comments.AddAsync(comments);
            await context.SaveChangesAsync();
            VideoClass current = await context.Reels.FirstOrDefaultAsync(x => x.Id == Guid.Parse(Id));
            var obj = await buildInitialModal(null, current);
            return View("Reels", obj);
        }

        
    }
}
