using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TechWizWebApp.Data;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.RepositotyCustomer
{
    public class BlogsFERepo : IBlogsFE
    {
        private DecorVistaDbContext _context;
        public BlogsFERepo(DecorVistaDbContext context)
        {
            _context = context;
        }
        public async Task<CustomResult> GetAll()
        {
            try
            {
                var list = await _context.Blogs.Include(e => e.interior_designer).Where(e => e.status == true).Select(e => new BlogRes()
                {
                    id = e.id,
                    author = e.interior_designer != null ? e.interior_designer.first_name : "Decor Vista",
                    title = e.title,
                    content = e.content,
                    imageavatar = e.images,
                    create_at = e.CreatedDate,
                    designer_id = e.interior_designer_id,
                }).OrderByDescending(e => e.create_at).ToListAsync();
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

        public async Task<CustomResult> GetByDesigner(int designerId)
        {
            try
            {
                var data = await _context.Blogs.Where(e => e.interior_designer_id == designerId && e.status == true).OrderByDescending(e => e.CreatedDate).ToListAsync();
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
                    Message = "Server Error"

                };
            }
        }

        public async Task<CustomResult> GetById(int id)
        {
            try
            {
                var data = await _context.Blogs.Include(b=>b.interior_designer).SingleOrDefaultAsync(e => e.id == id && e.status == true);
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
                    Message = "Server Error"

                };
            }
        }

        private class BlogRes
        {
            public int? id { get; set; }
            public string? title { get; set; }

            public string? author { get; set; }

            public string? imageavatar { get; set; }

            public string? content { get; set; }

            public int? designer_id { get; set; }

            public DateTime? create_at { get; set; }
        }
    }
}
