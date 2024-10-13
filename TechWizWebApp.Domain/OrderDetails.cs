using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWizWebApp.Domain
{
    public class OrderDetails
    {
        public int id { get; set; }
        public string order_id { get; set; } = string.Empty;
        public Order? order { get; set; }
        public int variant_id { get; set; }
        public Variant? variant { get; set; }
        public int quanity {  get; set; }
        public bool review_status { get; set; } = false;
    }

}
