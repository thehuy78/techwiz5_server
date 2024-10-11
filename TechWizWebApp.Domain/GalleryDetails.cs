using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWizWebApp.Domain
{
    public class GalleryDetails
    {
        [Key]
        public int id { get; set; }
        public int gallery_id { get; set; }
        public Gallery? gallery { get; set; } 
        public int product_id { get; set; }
        public Product? product { get; set; } 
    }
}
