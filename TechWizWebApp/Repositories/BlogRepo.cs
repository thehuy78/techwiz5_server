using TechWizWebApp.Data;
using TechWizWebApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Domain;
using TechWizWebApp.Controllers;
using TechWizWebApp.Services;
using Firebase.Auth;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;
using TechWizWebApp.RequestModels;


namespace TechWizWebApp.Repositories
{
    public class BlogRepo : IBlog
    {
        private readonly ILogger<BlogRepo> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly IFileService _fileService;
        private readonly IMailService _mailService;
        protected DecorVistaDbContext _context;
        public BlogRepo(DecorVistaDbContext dataContext, ILogger<BlogRepo> logger, IConfiguration configuration, IWebHostEnvironment env, IFileService fileService, IMailService mailService)
        {
            _logger = logger;
            _config = configuration;
            _env = env;
            _fileService = fileService;
            _mailService = mailService;
            _context = dataContext;
        }
        public async Task<CustomResult> fitterStatus(bool? status, string? name)
        {
            try
            {

                if (status == null && name == null)
                {
                    var query = _context.Blogs
                       .AsNoTracking()
                       .Include(b => b.interior_designer)
                       .AsSingleQuery();

                    var result = await query.ToListAsync();
                    result.Reverse();


                    return new CustomResult(200, "Success", result);
                }
                if (status != null && name == null)
                {
                    var query = _context.Blogs.Where(b => b.status == status)
                    .AsNoTracking()
                    .Include(b => b.interior_designer)
                    .AsSingleQuery();

                    var result = await query.ToListAsync();
                    result.Reverse();
                    return new CustomResult(200, "Success", result);
                }
                if (status == null && name != null)
                {
                    var query = _context.Blogs.Where(b => b.title.Contains(name))
                    .AsNoTracking()
                    .Include(b => b.interior_designer)
                    .AsSingleQuery();

                    var result = await query.ToListAsync();
                    result.Reverse();
                    return new CustomResult(200, "Success", result);
                }
                if (status != null && name != null)
                {
                    var query = _context.Blogs.Where(b => b.status == status && b.title.Contains(name))
                    .AsNoTracking()
                    .Include(b => b.interior_designer)
                    .AsSingleQuery();

                    var result = await query.ToListAsync();
                    result.Reverse();
                    return new CustomResult(200, "Success", result);
                }

                return new CustomResult(400, "failed", null);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Success", ex.Message);

            }

        }
        public async Task<CustomResult> readBlogById(int id)

        {
            try
            {
                var blog = await _context.Blogs.Where(b => b.id == id).Include(b => b.interior_designer).SingleOrDefaultAsync();
                if (blog != null)
                {
                    return new CustomResult(200, "success", blog);
                }
                return new CustomResult(400, "Failed", null);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);

            }
        }
        public async Task<CustomResult> activeBlog(RequestActiveBlog request)
        {
            try
            {
                var blog = await _context.Blogs.Where(b => b.id == request.blogId).SingleOrDefaultAsync();
                if (blog != null)
                {
                    if (request.yes)
                    {
                        blog.status = true;
                    }
                    else
                    { blog.status = false; }
                    await _context.SaveChangesAsync();
                    return new CustomResult(200, "success", blog);
                }
                return new CustomResult(400, "Failed", null);


            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", ex.Message);

            }
        }

        public async Task<CustomResult> createBlog(int? user_id, Blog blog)
        {
            try
            {
                if (blog.imagesFile != null)
                {
                    string imageString = "";
                    foreach (var image in blog.imagesFile)
                    {
                        var result = await _fileService.UploadImageAsync(image);
                        imageString = result;
                    }
                    blog.images = imageString;
                }

                if (user_id == null)
                {
                    blog.interior_designer_id = null;
                    blog.status = true;
                }
                else
                {
                    var interiorDesigner = await _context.InteriorDesigners.SingleOrDefaultAsync(i => i.user_id == user_id);
                    blog.interior_designer_id = interiorDesigner.id;
                    blog.status = false;
                }

                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();
                return new CustomResult(200, "success", blog);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Erorr", ex.Message);
            }
        }

        public async Task<CustomResult> readAll()
        {
            try
            {
                var query = _context.Blogs
                    .AsNoTracking()
                    .AsSingleQuery();

                var result = await query.ToListAsync();
                result.Reverse();

                return new CustomResult(200, "Success", result);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", null);
            }
        }

        public async Task<CustomResult> readBlogByDesigner(int? ownerBlogId)
        {
            try
            {

                var query = _context.Blogs
                    .AsNoTracking()
                .AsSingleQuery();

                var result = await query.Where(b => b.interior_designer_id == ownerBlogId).ToListAsync();


                return new CustomResult(200, "Success", result);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", null);
            }
        }

        public async Task<CustomResult> searchBlogByName(string name)
        {
            try
            {

                var query = _context.Blogs
                    .AsNoTracking()
                .AsSingleQuery();

                var result = await query.Where(b => b.title.Contains(name)).ToListAsync();



                return new CustomResult(200, "Success", result);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", null);
            }
        }

        public async Task<CustomResult> updateBlog(Blog blog)
        {
            try
            {
                var oldBlog = await _context.Blogs.SingleOrDefaultAsync(b => b.id == blog.id);

                if (blog.imagesFile != null)
                {
                    string imageString = "";
                    foreach (var image in blog.imagesFile)
                    {
                        var fileName = "==" + DateTime.Now.Ticks + image.FileName;
                        var result = await _fileService.UploadImageAsync(image);
                        imageString += fileName;

                    }
                    oldBlog.images = imageString;

                }
                oldBlog.content = blog.content;
                oldBlog.title = blog.title;
                oldBlog.status = false;
                _context.Blogs.Update(oldBlog);
                return new CustomResult(200, "success", oldBlog);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", null);

            }
        }
    }


}


