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
        public async Task<CustomPaging> getBlogByAdmin(int pageNumber, int pageSize, bool status, List<string> by, string designerName, string name)
        {
            try
            {
                IQueryable<Blog> query;

                query = _context.Blogs;

                query = query.Include(p => p.interior_designer).Where(b => b.status == status && b.title.Contains(name)).OrderByDescending(c => c.UpdatedDate);

                if (by.Count != 0 && by.Count != 2)
                {
                    if (by.Contains("Designer"))
                    {
                        query = query.Where(p => p.interior_designer != null);
                    }
                    else
                    {
                        query = query.Where(p => p.interior_designer == null);
                    }
                }

                if (designerName.Length > 0)
                {
                    query = query.Where(p => p.interior_designer != null && (p.interior_designer.first_name.Contains(designerName) || p.interior_designer.last_name.Contains(designerName)));
                }

                var total = query.Count();

                query = query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

                var list = await query.ToListAsync();

                var customPaging = new CustomPaging()
                {
                    Status = 200,
                    Message = "OK",
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)total / pageSize),
                    PageSize = pageSize,
                    TotalCount = total,
                    Data = list
                };

                return customPaging;
            }
            catch (Exception ex)
            {
                return new CustomPaging()
                {
                    Status = 400,
                    Message = ex.Message,
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    PageSize = pageSize,
                    TotalCount = 0,
                    Data = null
                };

            }
        }

        public async Task<CustomPaging> getBlogByDesigner(int userId, int pageNumber, int pageSize, bool status, string name)
        {
            try
            {
                var designer = await _context.InteriorDesigners.SingleOrDefaultAsync(p => p.user_id == userId);

                if (designer == null)
                {
                    return new CustomPaging()
                    {
                        Status = 404,
                        Message = "Not found",
                        CurrentPage = pageNumber,
                        TotalPages = 0,
                        PageSize = pageSize,
                        TotalCount = 0,
                        Data = null
                    };
                }

                IQueryable<Blog> query;

                query = _context.Blogs;

                query = query.Include(p => p.interior_designer).Where(b => b.interior_designer_id == designer.id && b.status == status && b.title.Contains(name)).OrderByDescending(c => c.UpdatedDate);


                var total = query.Count();

                query = query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

                var list = await query.ToListAsync();

                var customPaging = new CustomPaging()
                {
                    Status = 200,
                    Message = "OK",
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)total / pageSize),
                    PageSize = pageSize,
                    TotalCount = total,
                    Data = list
                };

                return customPaging;
            }
            catch (Exception ex)
            {
                return new CustomPaging()
                {
                    Status = 400,
                    Message = ex.Message,
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    PageSize = pageSize,
                    TotalCount = 0,
                    Data = null
                };

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
                var blog = await _context.Blogs.Include(b => b.interior_designer).Where(b => b.id == request.blogId).SingleOrDefaultAsync();
                if (blog != null)
                {
                    if (request.yes)
                    {
                        blog.status = true;
                        if (blog.interior_designer != null)
                        {
                            var newNotification = new Notification
                            {
                                created_date = DateTime.Now,
                                is_read = false,
                                message = $@"Admin has active your blog",
                                type = "designer:blog",
                                url = "/getblogbyid?id=" + blog.id,
                                user_id = blog.interior_designer.user_id
                            };
                            _context.Notifications.Add(newNotification);
                    }

                    }
                    else
                    {
                        blog.status = false;

                        if (blog.interior_designer != null)
                        {
                            var newNotification = new Notification
                            {
                                created_date = DateTime.Now,
                                is_read = false,
                                message = $@"Admin has deactive your blog",
                                type = "designer:blog",
                                url = "/getblogbyid?id=" + blog.id,
                                user_id = blog.interior_designer.user_id
                            };
                            _context.Notifications.Add(newNotification);

                        }
                    }
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
                    _context.Blogs.Add(blog);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var interiorDesigner = await _context.InteriorDesigners.SingleOrDefaultAsync(i => i.user_id == user_id);
                    blog.interior_designer_id = interiorDesigner.id;
                    blog.status = false;
                    _context.Blogs.Add(blog);
                    await _context.SaveChangesAsync();

                   
                    var admins = await _context.UserDetails.Where(u => u.role == "admin").ToListAsync();

                    foreach (var admin in admins)
                    {
                        var newNotification = new Notification
                        {
                            created_date = DateTime.Now,
                            is_read = false,
                            message = $@"Designer with an name {interiorDesigner.first_name + " " + interiorDesigner.last_name} has created new blog",
                            type = "admin:blog",
                            url = "/getblogbyid?id=" + blog.id,
                            user_id = admin.user_id
                        };

                        _context.Notifications.Add(newNotification);
                    }

                    await _context.SaveChangesAsync();
                }

        
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

        public async Task<CustomResult> UpdateBlog(RequestUpdateBlog requestUpdateBlog)
        {
            try
            {
                var blog = await _context.Blogs.SingleOrDefaultAsync(b => b.id == requestUpdateBlog.Id);

                if (blog == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                blog.title = requestUpdateBlog.Title;
                blog.content = requestUpdateBlog.Content;

                if (requestUpdateBlog.Image != null)
                {
                    blog.images = await _fileService.UploadImageAsync(requestUpdateBlog.Image);
                }

                _context.Blogs.Update(blog);

                await _context.SaveChangesAsync();

                if (blog.interior_designer_id != null)
                {
                    var designer = await _context.InteriorDesigners.SingleOrDefaultAsync(d => d.id == blog.interior_designer_id);
                    var admins = await _context.UserDetails.Where(u => u.role == "admin").ToListAsync();

                    foreach (var admin in admins)
                    {
                        var newNotification = new Notification
                        {
                            created_date = DateTime.Now,
                            is_read = false,
                            message = $@"Designer with an name {designer.first_name + " " + designer.last_name} has changed their blog ",
                            type = "admin:blog",
                            url = "/getblogbyid?id=" + blog.id,
                            user_id = admin.user_id
                        };

                        _context.Notifications.Add(newNotification);
                    }

                    await _context.SaveChangesAsync();
                }

                return new CustomResult(200, "Success", null);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Failed", null);
            }
        }
    }


}


