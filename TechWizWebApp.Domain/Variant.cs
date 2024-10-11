using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWizWebApp.Domain
{
    public class Variant
    {
        [Key]
        public int id { get; set; }
        public int productid { get; set; }
        public Product? product { get; set; }
        public float price { get; set; }
        public float saleprice { get; set; }
        public List<VariantAttribute> variantattributes { get; set; } = new List<VariantAttribute>();

        public List<OrderDetails> order_details { get; set; } = new List<OrderDetails>();
        public List<Cart> carts { get; set; } = new List<Cart>();
    }
}
