using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;

namespace TechWizWebApp.Domain
{
    public class User
    {
        [Key]
        public int id { get; set; }
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public UserDetails? userdetails { get; set; }
        public InteriorDesigner? interiordesigner { get; set; }
        public List<Consultation> consultations { get; set; } = new List<Consultation>();
        public List<Review> reviews { get; set; } = new List<Review>();
        public List<Cart> carts { get; set; } = new List<Cart>();
        public List<Order> orders { get; set; } = new List<Order>();
        public List<Subcribe> subcribes { get; set; } = new List<Subcribe>();

        public List<Notification> notifications { get; set; } = new List<Notification>();

        [NotMapped]
        public string Role
        {
            get
            {
                var userRole = "";

                if(userdetails != null && userdetails.role == "admin")
                {
                    userRole = "admin";
                }
                else
                {
                    userRole = "customer";
                }

                if(userdetails == null && interiordesigner != null)
                {
                    userRole = "designer";
                }

                return userRole;
            }
        }
    }
}
