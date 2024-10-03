using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWizWebApp.Domain
{
    public class InteriorDesigner
    {
        [Key]        
        public int id { get; set; }
        public int user_id { get; set; }
        public User? user { get; set; }
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string contact_number { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public int yearsofexperience { get; set; } 
        public string specialization { get; set; } = string.Empty;
        public string portfolio { get; set; } = string.Empty;
        public string daywork { get; set; } = string.Empty;
        public string avatar {  get; set; } = string.Empty;
        public string? certificate { get; set; } = string.Empty;
        public bool status { get; set; }
        public string? approved_status {  get; set; }
        public List<Gallery> galleries { get; set; } = new List<Gallery>();
        public List<Review> reviews { get; set; } = new List<Review>();
        public List<Consultation> consultations { get; set; } = new List<Consultation>();
        public List<Blog> blogs { get; set; } = new List<Blog>();
        public List<Story> stories { get; set; } = new List<Story>();
    }
}
