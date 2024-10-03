using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWizWebApp.Domain
{
    public class Order
    {
        [Key]
        public string id { get; set; } = Guid.NewGuid().ToString();
        public float total { get; set; }
        public string address { get; set; } = string.Empty;
        public int user_id { get; set; }
        public User? user { get; set; }
        public string? status { get; set; } // packaged, delivery, completed
        public string? fullname { get; set; }

        public string? phone { get; set; }
        public List<OrderDetails> order_details { get; set; } = new List<OrderDetails>();
        public DateTime created_date { get; set; } = DateTime.Now;

        public DateTime updated_date { get; set; } = DateTime.Now;


    }

}