using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.Interfaces;
using TechWizWebApp.RequestModels;
using TechWizWebApp.Services;

namespace TechWizWebApp.Repositories
{
    public class ProductAdminRepo : IProductAdmin
    {

        private readonly DecorVistaDbContext _context;
        private readonly IConfiguration _config;
        private readonly IFileService _fileService;
        private readonly IMailService _mailService;

        public ProductAdminRepo(DecorVistaDbContext context, IConfiguration config, IFileService fileService, IMailService mailService)
        {
            _context = context;
            _config = config;
            _fileService = fileService;
            _mailService = mailService;
        }

        public async Task<CustomResult> ChangeProductStatus(int productId)
        {
            try
            {
                var product = await _context.Products.SingleOrDefaultAsync(p => p.id == productId);

                if (product == null)
                {
                    return new CustomResult(400, "Not found", null);
                }

                product.status = !product.status;

                _context.Products.Update(product);

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", null);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad Request", ex.Message);
            }
        }

        public async Task<CustomResult> CreateNewProduct(RequestCreateNewProduct requestCreateNewProduct)
        {
            try
            {
                var newProduct = new Product()
                {
                    brand = requestCreateNewProduct.Brand,
                    description = requestCreateNewProduct.Description,
                    productname = requestCreateNewProduct.ProductName,
                    functionality_id = requestCreateNewProduct.RoomFuncion,
                    status = requestCreateNewProduct.Status,
                };

                ICollection<Color> colors = new List<Color>();

                if (requestCreateNewProduct.colorJson != null)
                {
                    foreach (var json in requestCreateNewProduct.colorJson)
                    {
                        var detail = JsonConvert.DeserializeObject<Color>(json);
                        colors.Add(detail);
                    }
                }

                foreach (var image in requestCreateNewProduct.Images)
                {
                    var newImage = new ProductImage();

                    newImage.product = newProduct;
                    var newImageName = await _fileService.UploadImageAsync(image);

                    if (newProduct.imageName.Length == 0)
                    {
                        newProduct.imageName = newImageName;
                    }

                    newImage.imagename = newImageName;
                    _context.ProductImages.Add(newImage);
                }

                foreach (var json in requestCreateNewProduct.VariantsJSON)
                {
                    var detail = JsonConvert.DeserializeObject<VariantDetail>(json);
                    requestCreateNewProduct.VariantDetails.Add(detail);
                }

                foreach (var item in requestCreateNewProduct.VariantDetails)
                {
                    var newVariant = new Variant()
                    {
                        product = newProduct,
                        price = item.RealPrice,
                        saleprice = item.FakePrice,
                    };

                    for (var i = 0; i < requestCreateNewProduct.Variants.Count; i++)
                    {
                        var newVariantAttribute = new VariantAttribute()
                        {
                            variant = newVariant,
                            priority = i + 1,
                            attributetype = requestCreateNewProduct.Variants.ToArray()[i],
                            attributevalue = item.variant.ToArray()[i]
                        };

                        if (newVariantAttribute.attributetype == "Color")
                        {
                            newVariantAttribute.note = colors.FirstOrDefault(colorObj => colorObj.color == newVariantAttribute.attributevalue).hex;
                        }

                        _context.VariantAttributes.Add(newVariantAttribute);
                    }

                    _context.Variants.Add(newVariant);
                }

                _context.Products.Add(newProduct);

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", newProduct);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad Request", ex.InnerException.Message);
            }

        }

        public async Task<CustomPaging> GetProductList(int pageNumber, int pageSize, bool active, IEnumerable<int> functionalityId, IEnumerable<string> brand, string search)
        {
            try
            {
                IQueryable<Product> query;

                query = _context.Products;

                query = query.Where(p => p.status == active);

                if (search != null)
                {
                    query = query.Where(p => p.productname.Contains(search));
                }

                if (functionalityId.Count() != 0)
                {
                    query = query.Where(p => functionalityId.Contains(p.functionality_id));
                }

                if (brand.Count() != 0)
                {
                    query = query.Where(p => brand.Contains(p.brand));
                }

                query = query.OrderByDescending(p => p.created_at);

                var total = query.Count();

                query = query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

                var list = await query.Include(p => p.variants).Include(p => p.functionality).ToListAsync();

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

        public async Task<CustomResult> GetProductSelect()
        {
            try
            {
                var list = await _context.Products.Select(e => new ProductSelect()
                {
                    value = e.id,
                    label = e.productname
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
                    Message = ex.Message,

                };
            }
        }

        public async Task<CustomResult> GetSpecificProduct(ICollection<int> productId)
        {
            try
            {
                var products = await _context.Products.Include(p => p.variants).Where(p => productId.Contains(p.id)).ToListAsync();

                return new CustomResult(200, "Success", products);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad Request", ex.Message);
            }
        }

        public async Task<CustomResult> SearchProduct(string productName)
        {
            try
            {
                var products = await _context.Products.Where(p => p.productname.Contains(productName) && p.status == true).Take(10).ToListAsync();

                return new CustomResult(200, "Success", products);


            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad Request", ex.Message);
            }
        }

        private class ProductSelect()
        {
            public int? value { get; set; }

            public string? label { get; set; }
        }

        private class Color
        {
            public string color { get; set; }
            public string hex { get; set; }

        }

    }

}
