using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWizWebApp.Domain
{
    public class Cart
    {
        [Key]
        public int id { get; set; }
        public int user_id { get; set; }
        public User? user { get; set; }
        public int product_id { get; set; }

        public int variant_id { get; set; }
        public Variant? variant { get; set; }

        public Product? product { get; set; }
        public int quanity { get; set; }
    }
}
