
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.Services;
using TechWizWebApp.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TechWizWebApp.Repository
{
    public class GalleryRepo : IGallery
    {
        private DecorVistaDbContext _context;
        private IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IFileService _fileService;
        private readonly IMailService _mailService;

        public GalleryRepo(DecorVistaDbContext context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, IFileService fileService, IMailService mailService)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _fileService = fileService;
            _mailService = mailService;
        }


        public async Task<CustomResult> ChangeStatus(int id)
        {
            try
            {
                var data = await _context.Galleries.SingleOrDefaultAsync(e => e.id == id);
                data!.status = !data.status;
                _context.Galleries.Update(data);
                await _context.SaveChangesAsync();
                return new CustomResult()
                {
                    Status = 200,
                    Message = "Change Status Success!"
                };
            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    Message = "Server Error!" + ex.InnerException.Message,
                };
            }
        }

        public async Task<CustomResult> Create(TechWizWebApp.Domain.Gallery e)
        {
            try
            {
                e.created_date = DateTime.Now;
                foreach (var item in e.uploadImages)
                {
                    var filename = await _fileService.UploadImageAsync(item);

                    e.imageName = e.imageName + "; " + filename;
                }
                e.status = false;
                _context.Galleries.Add(e);

                foreach(var id in e.product_list)
                {
                    var newGalleryDetail = new GalleryDetails();

                    newGalleryDetail.gallery = e;
                    newGalleryDetail.product_id = id;

                    _context.GalleryDetails.Add(newGalleryDetail);
                }

                await _context.SaveChangesAsync();
                return new CustomResult()
                {
                    Status = 200,
                    Message = "Create Success."
                };
            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    Message = "Create Fail." + ex.InnerException?.Message
                };
            }
        }

        public async Task<CustomResult> GetAll()
        {
            try
            {
                var list = await _context.Galleries.Include(e => e.room_type).ToListAsync();
                return new CustomResult()
                {
                    Status = 200,
                    data = list
                };
            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    Message = "Server Error." + ex.InnerException?.Message
                };
            }
        }

        public async Task<CustomResult> GetById(int id)
        {
            try
            {
                var data = await _context.Galleries.SingleOrDefaultAsync(e => e.id == id);
                return new CustomResult()
                {
                    Status = 200,
                    data = data
                };
            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    Message = "Server Error!"
                };
            }
        }

        public Task<CustomResult> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomResult> Update(TechWizWebApp.Domain.Gallery e)
        {
            try
            {

                var oldData = await _context.Galleries.SingleOrDefaultAsync(a => a.id == e.id);
                if (e.uploadImages != null)
                {
                    foreach (var item in e.uploadImages)
                    {
                        var filename = await _fileService.UploadImageAsync(item);

                        e.imageName = e.imageName + "; " + filename;
                    }
                }

                oldData.id = e.id;
                oldData.imageName = e.imageName;
                oldData.gallery_name = e.gallery_name;
                oldData.updated_date = DateTime.Now;
                oldData.description = e.description;
                oldData.color_tone = e.color_tone;
                oldData.room_type_id = e.room_type_id;
                _context.Galleries.Update(oldData);
                await _context.SaveChangesAsync();
                return new CustomResult()
                {
                    Status = 200,
                    Message = "Update Success."
                };
            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    Message = "Update Fail." + ex.InnerException?.Message
                };
            }
        }
        public string GetUniqueFilename(string file)
        {
            file = Path.GetFileName(file);
            return Path.GetFileNameWithoutExtension(file) + "_" + DateTime.Now.Ticks + Path.GetExtension(file);
        }
    }
}
