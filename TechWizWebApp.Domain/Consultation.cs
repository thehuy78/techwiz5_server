using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWizWebApp.Domain
{
    public class Consultation
    {
        [Key]        
        public int id { get; set; }
        public string? time { get; set; }
        public int user_id { get; set; }
        public User? user { get; set; } 
        public int designer_id { get; set; }
        public InteriorDesigner? interior_designer { get; set; }
        public DateTime scheduled_datetime { get; set; }
        public string address {  get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string notes { get; set; } = string.Empty;

        public Review? review { get; set; }
    }
}
