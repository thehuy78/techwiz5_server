using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.Interfaces;
using TechWizWebApp.Services;

namespace TechWizWebApp.Repositories
{
    public class StoryAdminRepo : IStoryAdmin
    {
        private readonly DecorVistaDbContext _context;
        private readonly IConfiguration _config;
        private readonly IFileService _fileService;
        private readonly IMailService _mailService;

        public StoryAdminRepo(DecorVistaDbContext context, IConfiguration config, IFileService fileService, IMailService mailService)
        {
            _context = context;
            _config = config;
            _fileService = fileService;
            _mailService = mailService;
        }

        public async Task<CustomResult> CreateNewStory(Story story)
        {
            try
            {

                foreach (var imageFile in story.upload_images)
                {
                    var imageName = await _fileService.UploadImageAsync(imageFile);
                    story.image = story.image + "; " + imageName;
                }

                story.created_at = DateTime.Now;

                _context.Stories.Add(story);

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", null);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<CustomPaging> GetDesignerStories(int designer_id, int pageNumber, int pageSize, string from, string to)
        {
            try
            {
                IQueryable<Story> query;

                query = _context.Stories;

               

                query = query.Where(s => s.interior_designer_id == designer_id ).OrderByDescending(s => s.created_at);

                if (from.Length != 0)
                {
                    DateTime fromDate = DateTime.Parse(from);
                    query = query.Where(s => s.created_at.Date >= fromDate);
                }

                if (to.Length != 0)
                {
                    DateTime toDate = DateTime.Parse(to);
                    query = query.Where(s => s.created_at.Date <= toDate);
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

        public async Task<CustomResult> UpdateStory(Story story)
        {
            try
            {
                var oldStory = await _context.Stories.SingleOrDefaultAsync(s => s.id == story.id);

                if (oldStory == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                oldStory.content = story.content;
                oldStory.image = "";

                if (story.old_images != null)
                {
                    foreach (var image in story.old_images)
                    {
                        oldStory.image = oldStory.image + "; " + image;
                    }
                }

                if (story.upload_images != null)
                {
                    foreach (var imageFile in story.upload_images)
                    {
                        var image = await _fileService.UploadImageAsync(imageFile);
                        oldStory.image = oldStory.image + "; " + image;
                    }
                }

                _context.Stories.Update(oldStory);

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", story);

            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }
    }
}
