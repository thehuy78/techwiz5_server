using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWizWebApp.Domain
{
    public class RoomType
    {
        [Key] 
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public List<Gallery> galleries { get; set; } = new List<Gallery>();

        public string? url {  get; set; }

    }
}
