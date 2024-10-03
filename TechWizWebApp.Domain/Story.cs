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
    public class Story
    {
        [Key]
        public int id { get; set; }

        public int interior_designer_id { get; set; }

        public string image { get; set; } = string.Empty;

        public string content { get; set; }

        public DateTime created_at { get; set; }

        public InteriorDesigner? interior_designer { get; set; }

        [NotMapped]
        public ICollection<string>? old_images { get; set; }

        [NotMapped]
        public ICollection<IFormFile>? upload_images { get; set; }
    }
}
