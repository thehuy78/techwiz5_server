using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Cryptography;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.RepositotyCustomer
{
    public class DesignerFERepo : IDesignerFE
    {
        private DecorVistaDbContext _context;
        public DesignerFERepo(DecorVistaDbContext context)
        {
            _context = context;
        }
        public async Task<CustomResult> GetAll()
        {
            try
            {
                var list = await _context.InteriorDesigners.Include(e => e.reviews).Include(e => e.blogs).Where(e => e.status == true).Select(e => new DesignerRes()
                {
                    id = e.id,
                    user_id = e.user_id,
                    first_name = e.first_name,
                    last_name = e.last_name,
                    contact_number = e.contact_number,
                    address = e.address,
                    yearsofexperience = e.yearsofexperience,
                    specialization = e.specialization,
                    portfolio = e.portfolio,
                    daywork = e.daywork,
                    status = e.status,
                    avatar = e.avatar,
                    count_review = e.reviews.Count(),
                    count_blog = e.blogs.Count(),
                    count_booking = e.consultations.Count(),
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
                    Message = "Server Error!"
                };
            }
        }

        public async Task<CustomResult> GetById(int id)
        {
            try
            {
                var list = await _context.InteriorDesigners.Include(e=>e.stories).Include(e => e.reviews).Include(e => e.blogs).Where(e => e.status == true).Select(e => new DesignerRes()
                {
                    id = e.id,
                    user_id = e.user_id,
                    first_name = e.first_name,
                    last_name = e.last_name,
                    contact_number = e.contact_number,
                    address = e.address,
                    yearsofexperience = e.yearsofexperience,
                    specialization = e.specialization,
                    portfolio = e.portfolio,
                    daywork = e.daywork,
                    status = e.status,
                    avatar = e.avatar,
                    certificate = e.certificate,
                    blogs = e.blogs.OrderByDescending(s => s.CreatedDate).ToList(),
                    stories = e.stories.OrderByDescending(s => s.created_at).ToList(),
                    count_review = e.reviews.Count(),
                    count_blog = e.blogs.Count(),
                    count_booking = e.consultations.Count(),
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
                    Message = "Server Error!"
                };
            }
        }

        private class DesignerRes()
        {
            public int id { get; set; }

            public int user_id { get; set; }

            public string first_name { get; set; }
            public string last_name { get; set; }
            public string contact_number { get; set; }
            public string address { get; set; }

            public string certificate { get; set; }
            public int yearsofexperience { get; set; }
            public string specialization { get; set; }
            public string portfolio { get; set; }
            public string daywork { get; set; }
            public bool status { get; set; }
             public string avatar { get; set; }
            public int count_review { get; set; }

            public List<Story>  stories { get; set; }

            public List<Blog> blogs { get; set; }

            public int count_booking { get; set; }

            public int count_blog { get; set; }
        }
    }
}
