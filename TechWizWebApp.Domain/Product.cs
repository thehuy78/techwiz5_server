using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TechWizWebApp.Domain
{
    public class Product
    {
        [Key]
        public int id { get; set; }
        public string product_code { get; set; } = string.Empty;
        public string productname { get; set; } = string.Empty;
        public int functionality_id { get; set; }
        public Functionality? functionality { get; set; } 
        public string brand { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string imageName { get; set; } = string.Empty;
        public DateTime created_at {  get; set; } = DateTime.Now;
        public DateTime updated_at { get; set; } = DateTime.Now;
        public bool status { get; set; }
        public List<Review> reviews { get; set; } = new List<Review>();
        public List<GalleryDetails> galleryDetails { get; set; } = new List<GalleryDetails>();
        public List<Cart> carts { get; set; } = new List<Cart>();
        public List<Variant> variants { get; set; } = new List<Variant>();
        public List<ProductImage> images { get; set; } = new List<ProductImage>();
        [NotMapped]
        public List<IFormFile>? uploadImages { get; set; }

        [NotMapped]
        public string _productCode
        {
            get
            {
                return GetProductCode();
            }
        }
        public string GetProductCode()
        {
            string productCode = $"P_{functionality?.name}_{brand}_{id}";
            return productCode;
        }
    }
}
