﻿
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.Services;
using TechWizWebApp.Interface;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore.Query;

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

        public async Task<CustomResult> Create(int userId, TechWizWebApp.Domain.Gallery e)
        {
            try
            {
                var user = await _context.Users.Include(u => u.userdetails).Include(u => u.interiordesigner).SingleOrDefaultAsync(u => u.id == userId);

                if (user == null)
                {
                    return new CustomResult(404, "Not found", null);
                }


                e.created_date = DateTime.Now;
                e.updated_date = DateTime.Now;

                foreach (var item in e.uploadImages)
                {
                    var filename = await _fileService.UploadImageAsync(item);

                    e.imageName = e.imageName + "; " + filename;
                }

                if (user.Role == "admin")
                {
                    e.status = true;
                }

                if (user.Role == "designer")
                {
                    e.status = false;
                    e.interior_designer_id = user.interiordesigner.id;
                }

                _context.Galleries.Add(e);

                foreach (var id in e.product_list)
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
                var data = await _context.Galleries.Include(p => p.galleryDetails).SingleOrDefaultAsync(e => e.id == id);
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

                e.imageName = "";

                if (e.oldImages != null)
                {
                    foreach (var image in e.oldImages)
                    {
                        e.imageName = e.imageName + "; " + image;
                    }
                }


                if (e.uploadImages != null)
                {
                    foreach (var item in e.uploadImages)
                    {
                        var filename = await _fileService.UploadImageAsync(item);

                        e.imageName = e.imageName + "; " + filename;
                    }
                }

                var oldProductList = await _context.GalleryDetails.Where(g => g.gallery_id == e.id).ToListAsync();

                foreach (var item in oldProductList)
                {
                    _context.GalleryDetails.Remove(item);
                }

                foreach (var id in e.product_list)
                {
                    var newGalleryDetail = new GalleryDetails();

                    newGalleryDetail.gallery_id = e.id;
                    newGalleryDetail.product_id = id;

                    _context.GalleryDetails.Add(newGalleryDetail);
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
                    Message = "Update Fail." + ex.Message
                };
            }
        }
        public string GetUniqueFilename(string file)
        {
            file = Path.GetFileName(file);
            return Path.GetFileNameWithoutExtension(file) + "_" + DateTime.Now.Ticks + Path.GetExtension(file);
        }

        public async Task<CustomPaging> GetAdminGalleries(int pageNumber, int pageSize, bool status, List<string> by, string designerName, string colorTone, string name)
        {
            try
            {
                IQueryable<Gallery> query;

                query = _context.Galleries;

                query = query.Include(p => p.interior_designer).Include(g => g.galleryDetails).Where(b => b.status == status && b.gallery_name.Contains(name)).OrderByDescending(c => c.updated_date);

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

                if (colorTone.Length > 0)
                {
                    query = query.Where(g => g.color_tone.Contains(colorTone));
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

        public async Task<CustomPaging> GetDesignerGalleries(int userId, int pageNumber, int pageSize, bool status, string colorTone, string name)
        {
            try
            {
                var designer = await _context.InteriorDesigners.SingleOrDefaultAsync(p => p.user_id == userId);

                if (designer == null)
                {
                    return new CustomPaging()
                    {
                        Status = 404,
                        Message = "Designer not found",
                        CurrentPage = pageNumber,
                        TotalPages = 0,
                        PageSize = pageSize,
                        TotalCount = 0,
                        Data = null
                    };
                }

                IQueryable<Gallery> query;

                query = _context.Galleries;

                query = query.Include(p => p.interior_designer).Include(g => g.galleryDetails).Where(b => b.interior_designer_id == designer.id && b.status == status && b.gallery_name.Contains(name)).OrderByDescending(c => c.updated_date);


                if (colorTone.Length > 0)
                {
                    query = query.Where(g => g.color_tone.Contains(colorTone));
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
    }
}
