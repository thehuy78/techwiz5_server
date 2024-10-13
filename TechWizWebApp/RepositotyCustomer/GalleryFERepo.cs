using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TechWizWebApp.Data;
using TechWizWebApp.InterfaceCustomer;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TechWizWebApp.RepositotyCustomer
{
    public class GalleryFERepo : IGalleryFE
    {
        private DecorVistaDbContext _context;
        public GalleryFERepo(DecorVistaDbContext context)
        {
            _context = context;
        }
        public async Task<CustomResult> GetAll()
        {
            try
            {
                var list = await _context.Galleries.Include(e => e.room_type).Include(e=>e.galleryDetails).Include(e => e.subcribes).Where(e => e.status == true).Select(e => new GalleryRes()
                {
                    id = e.id,
                    name = e.gallery_name,
                    description = e.description,
                    status = e.status,
                    room_type_id = e.room_type_id,
                    color_tone = e.color_tone,
                    view_count = e.view_count,
                    imageName = e.imageName,
                    created_date = e.created_date,
                    updated_date = e.updated_date,
                    subcribes = e.subcribes.Count(),
                    roomtype = e.room_type.name,
                    stringurl = e.room_type.url,
                    product_count = e.galleryDetails.Count()
                }).ToListAsync();

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
                    Message = "Server Error"
                };
            }
        }

        public async Task<CustomResult> GetById(int id)
        {
            try
            {
                var list = await _context.Galleries.Include(e => e.subcribes).Include(e=>e.galleryDetails).Where(e => e.status == true).Select(e => new GalleryRes()
                {
                    id = e.id,
                    name = e.gallery_name,
                    description = e.description,
                    status = e.status,
                    room_type_id = e.room_type_id,
                    color_tone = e.color_tone,
                    view_count = e.view_count,
                    imageName = e.imageName,
                    created_date = e.created_date,
                    updated_date = e.updated_date,
                    subcribes = e.subcribes.Count(),
                    roomtype = e.room_type.name,
                     product_count = e.galleryDetails.Count()

                }).SingleOrDefaultAsync(e => e.id == id);

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
                    Message = "Server Error"
                };
            }
        }

        public async Task<CustomResult> GetByRoomType(int idRoomType)
        {
            try
            {
                var list = await _context.Galleries.Include(e => e.subcribes).Include(e=>e.galleryDetails).Where(e => e.status == true).Select(e => new GalleryRes()
                {
                    id = e.id,
                    name = e.gallery_name,
                    description = e.description,
                    status = e.status,
                    room_type_id = e.room_type_id,
                    color_tone = e.color_tone,
                    view_count = e.view_count,
                    imageName = e.imageName,
                    created_date = e.created_date,
                    updated_date = e.updated_date,
                    subcribes = e.subcribes.Count(),
                    product_count = e.galleryDetails.Count(),
                    roomtype = e.room_type.name
                }).Where(e => e.room_type_id == idRoomType).ToListAsync();

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
                    Message = "Server Error"
                };
            }
        }

        public async Task<CustomResult> GetByRoomTypeUrl(string url)
        {
            try
            {
                var list = await _context.Galleries.Include(e => e.subcribes).Include(e => e.room_type).Include(e=>e.galleryDetails).Where(e => e.room_type.url == url && e.status == true).OrderByDescending(e => e.created_date).Select(e => new GalleryRes()
                {
                    id = e.id,
                    name = e.gallery_name,
                    description = e.description,
                    status = e.status,
                    room_type_id = e.room_type_id,
                    color_tone = e.color_tone,
                    view_count = e.view_count,
                    imageName = e.imageName,
                    created_date = e.created_date,
                    updated_date = e.updated_date,
                    roomtype = e.room_type.name,
                    subcribes = e.subcribes.Count(),
                    product_count = e.galleryDetails.Count()
                }).ToListAsync();
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
                    Message = "Server Error"
                };
            }
        }

        public async Task<CustomResult> GetNewest(string url)
        {
            try
            {
                var data = await _context.Galleries.Include(e => e.room_type).Where(e => e.room_type.url == url && e.status == true).OrderByDescending(e => e.created_date).FirstOrDefaultAsync();
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

        private class GalleryRes()
        {
            public int id { get; set; }
            public string name { get; set; }

            public string description { get; set; }

            public bool status { get; set; }

            public int room_type_id { get; set; }

            public string color_tone { get; set; }

            public int? view_count { get; set; }

            public int? product_count { get; set; }

            public string imageName { get; set; }

            public string? roomtype { get; set; }

            public int? subcribes { get; set; }

            public string? stringurl { get; set; }

            public DateTime? created_date { get; set; }

            public DateTime? updated_date { get; set; }

        }
    }
}
