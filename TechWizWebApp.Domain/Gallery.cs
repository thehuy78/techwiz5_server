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
    public class Gallery
    {
        [Key]
        public int id { get; set; }
        public string gallery_name { get; set; }= string.Empty;
        public string description { get; set; } = string.Empty;
        public bool status { get; set; }
        public int room_type_id { get; set; }

        public int? view_count {  get; set; }
        public int? interior_designer_id { get; set; }
        public InteriorDesigner? interior_designer { get; set; }

        public string color_tone {  get; set; } = string.Empty;

        public string imageName { get; set; } = string.Empty;
        public RoomType? room_type { get; set; }
        public List<GalleryDetails>? galleryDetails { get; set; } 
        public List<Subcribe>? subcribes { get; set; }  
        
        [NotMapped]
        public List<IFormFile>? uploadImages { get; set; }

        [NotMapped]
        public List<string>? oldImages { get; set; }

        public DateTime? created_date { get; set; } 

        public DateTime? updated_date { get; set;}

        [NotMapped]
        public List<int>? product_list { get; set; }
    }
}
