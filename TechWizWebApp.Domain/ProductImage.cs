using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWizWebApp.Domain
{
    public class ProductImage
    {
        [Key]
        public int id { get; set; }
        public int productid { get; set; }
        public Product? product { get; set; }
        public string? imagename { get; set; }
    }
}
