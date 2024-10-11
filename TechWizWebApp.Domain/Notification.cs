using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechWizWebApp.Domain
{
    public class Notification
    {
        [Key]
        public int id { get; set; }

        public int user_id { get; set; }

        public bool is_read { get; set; } = false;

        public string type { get; set; }

        public string message { get; set; }

        public string url { get; set; }

        public DateTime created_date { get; set; }

        public User? user { get; set; }

    }
}
