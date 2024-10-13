using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.RepositotyCustomer
{
    public class OrderDetailFERepo : IOrderdetailFE
    {
        private DecorVistaDbContext _dbContext;

        public OrderDetailFERepo(DecorVistaDbContext context)
        {
            _dbContext = context;
        }
        public async Task<CustomResult> getByOrderId(string orderid)
        {
            try
            {

                var list = await _dbContext.OrderDetails.Include(e=>e.order).Include(e => e.variant).ThenInclude(e => e.variantattributes).Where(e => e.order_id == orderid).Select(e => new OrderDetailRes()
                {
                    id = e.id,
                    fullname = e.order.fullname,
                    phone = e.order.phone,
                    address = e.order.address,
                    create_at = e.order.created_date,
                    variant = e.variant,
                    price = e.variant.price,
                    quantity = e.quanity,
                    idorder = orderid,
                    status = e.order.status,
                    variantattributes = e.variant.variantattributes,
                    image = e.variant.product.imageName,
                    totalprice = e.order.total,

                    productname = e.variant.product.productname,
                    idProduct = e.variant.product.id


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
    }

    public class OrderDetailRes
    {
        public int? id { get; set; }
        public string? idorder { get; set; }

        public string? status { get; set; }

        public string? image { get; set; }

        public int? idProduct { get; set; }

        public string? productname { get; set; }
        public string? fullname { get; set; }

        public List<VariantAttribute>? variantattributes { get; set; }

        public string? address { get; set; }

        public int? quantity { get; set; }

        public string? phone { get; set; }

        public DateTime? create_at { get; set; }

        public Variant? variant { get; set; }

        public float? price { get; set; }

        public float? totalprice { get; set; }


    }

}
