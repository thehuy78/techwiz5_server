
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TechWizWebApp.Data;
using TechWizWebApp.Services;
using TechWizWebApp.Interface;

namespace TechWizWebApp.Repository
{
    public class OrderDetailRepo : IOrderDetails
    {
        private DecorVistaDbContext _dbContext;
        private readonly IFileService _fileService;
        public OrderDetailRepo(DecorVistaDbContext decorVistaDbContext, IFileService fileService)
        {
                _dbContext = decorVistaDbContext;
                _fileService = fileService;
        }
        public async Task<CustomResult> GetByOrderId(string orderId)
        {
            try {
                var list = await _dbContext.OrderDetails.Where(e => e.order_id == orderId).Include(e => e.variant).ThenInclude(e => e.variantattributes).Include(e => e.variant).ThenInclude(e => e.product).Select(e => new OrderDetailsRes()
                {
                    id = e.id,
                    image = e.variant.product.imageName,
                    orderId = orderId,
                    price = e.variant.price,
                    product = e.variant.product.productname,
                    type = e.variant.variantattributes
                }).ToListAsync();
                return new CustomResult()
                {
                    Status = 200,
                    data = list
                };
            }
            catch(Exception ex) {
                return new CustomResult()
                {
                    Status = 400,
                   Message = ex.Message
                };
            }
        }

        private class OrderDetailsRes
        {
            public int id { get; set; }
            public string orderId { get; set; }
            public string product { get; set; }

            public float price { get; set; }
           
            public List<TechWizWebApp.Domain.VariantAttribute> type { get; set; }

            public string image { get; set; }
        }

    }
}
