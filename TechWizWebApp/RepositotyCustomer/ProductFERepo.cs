
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.InterfaceCustomer;


namespace TechWizWebApp.RepositotyCustomer
{
    public class ProductFERepo : IProductFE
    {
        private DecorVistaDbContext _context;
        public ProductFERepo(DecorVistaDbContext context)
        {
            _context = context;
        }
        public async Task<CustomResult> GetAll()
        {
            try
            {


                var list = await _context.Products.Include(e => e.reviews).Include(e => e.variants).ThenInclude(e => e.order_details).Where(e => e.status == true).Select(e => new ProductRes()
                {
                    id = e.id,
                    brand = e.brand,
                    price = e.variants.Min(v => v.price),
                    sale_count = e.variants.Count(variant => variant.order_details.Any()),
                    create_date = e.created_at,
                    productCode = e.product_code,
                    productName = e.productname,
                    imageName = e.imageName,
                    functionality_id = e.functionality_id,
                    description = e.description,
                    status = e.status,
                    score = e.reviews.Average(e => e.score),
                    functionality_name = e.functionality.name
                }).OrderByDescending(e => e.create_date).ToListAsync();


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

                var data = await _context.Products.Include(e => e.reviews).Include(e => e.variants).ThenInclude(e => e.variantattributes).Where(e => e.status == true).Select(e => new ProductRes()
                {
                    id = e.id,
                    brand = e.brand,
                    price = e.variants.Min(v => v.price),
                    create_date = e.created_at,
                    sale_count = e.variants.Count(variant => variant.order_details.Any()),
                    productCode = e.product_code,
                    productName = e.productname,
                    imageName = e.imageName,
                    listimage = e.images,
                    functionality_id = e.functionality_id,
                    description = e.description,
                    status = e.status,
                    score = e.reviews.Average(e => e.score),
                    functionality_name = e.functionality.name,
                    variants = e.variants,
                    listreview = e.reviews,

                }).SingleOrDefaultAsync(e => e.id == id);


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

        public async Task<CustomResult> GetProductByGallery(int idGallery)
        {
            try
            {
                var list = await _context.GalleryDetails.Include(e => e.product).ThenInclude(e => e.reviews).ThenInclude(e=>e.user).ThenInclude(e=>e.userdetails).Include(e => e.product).ThenInclude(e => e.variants).ThenInclude(e => e.variantattributes).Where(e => e.gallery_id == idGallery).Select(e => new ProductRes()
                {
                    id = e.product.id,
                    brand = e.product.brand,
                    price = e.product.variants.Min(v => v.price),
                    create_date = e.product.created_at,
                    sale_count = e.product.variants.Count(variant => variant.order_details.Any()),
                    productCode = e.product.product_code,
                    productName = e.product.productname,
                    imageName = e.product.imageName,
                    listimage = e.product.images,
                    functionality_id = e.product.functionality_id,
                    description = e.product.description,
                    status = e.product.status,
                    score = e.product.reviews.Average(e => e.score),
                    functionality_name = e.product.functionality.name,
                    variants = e.product.variants,

                }).ToListAsync();

                return new CustomResult() { Status = 200, data = list };
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

        private class ProductRes
        {
            public int? id { get; set; }

            public List<Variant> variants { get; set; }

            public string? firstnameuser { get; set; }

            public DateTime? create_date { get; set; }

            public List<ProductImage> listimage { get; set; }

            public int? sale_count { get; set; }

            public string? productCode { get; set; }

            public string? productName { get; set; }

            public int? functionality_id { get; set; }

            public string? functionality_name { get; set; }

            public string? brand { get; set; }

            public float? price { get; set; }
            public string? description { get; set; }

            public string? imageName { get; set; }

            public bool? status { get; set; }
            public float? score { get; set; }

            public List<Review>? listreview { get; set; }
        }

    }
}
