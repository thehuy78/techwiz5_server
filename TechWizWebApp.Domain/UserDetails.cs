using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWizWebApp.Domain
{
    public class UserDetails
    {
        [Key]        
        public int id { get; set; }
        public int user_id { get; set; }
        public User? User { get; set; }
        public string role { get; set; } = string.Empty;
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string contact_number { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string avatar {  get; set; } = string.Empty;
    }
}
