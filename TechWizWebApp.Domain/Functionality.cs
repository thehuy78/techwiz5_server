using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWizWebApp.Domain
{
    public class Functionality
    {
        [Key]        
        public int id { get; set; }
        public string name { get; set; } =string.Empty;
        public List<Product> products { get; set; } = new List<Product>();

        [NotMapped]
        public int? productCount
        {
            get
            {
                return products?.Count;
            }
        }
    }
}


