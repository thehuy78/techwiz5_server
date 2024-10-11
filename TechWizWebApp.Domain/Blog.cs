using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWizWebApp.Domain
{
    public class Blog
    {
        [Key]
        public int id { get; set; }
        public int? interior_designer_id { get; set; }
        public InteriorDesigner? interior_designer { get; set; } 
        public string title { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public string images { get; set; } = string.Empty;
        public bool status { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        [NotMapped]
        public List<IFormFile> imagesFile { get; set; }

    }
}
