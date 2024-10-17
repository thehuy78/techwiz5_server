
using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.Interface;
using TechWizWebApp.Services;

namespace TechWizWebApp.Repository
{
    public class OrderRepo : IOrder
    {
        private readonly DecorVistaDbContext _context;
        private readonly IMailService _mailService;

        public OrderRepo(DecorVistaDbContext dbContext, IMailService mailService )
        {
            _context = dbContext;
            _mailService = mailService;
        }

        public async Task<CustomResult> ChangeStatus(string id)
        {
            try
            {
                var data = await _context.Orders.Include(p => p.user).SingleOrDefaultAsync(e => e.id == id);
                if (data.status == "packaged")
                {
                    var newNotification = new Notification
                    {
                        created_date = DateTime.Now,
                        is_read = false,
                        message = $@"Your order with id {data.id} has been delivery",
                        type = "customer:order",
                        url = "/orderdetails/" + data.id,
                        user_id = data.user.id
                    };
                    _context.Notifications.Add(newNotification);

                    data.status = "delivery";
                }
                else
                {
                    var newNotification = new Notification
                    {
                        created_date = DateTime.Now,
                        is_read = false,
                        message = $@"Your order with id {data.id} has been completed",
                        type = "customer:order",
                        url = "/orderdetails/" + data.id,
                        user_id = data.user.id
                    };
                    _context.Notifications.Add(newNotification);
                    data.status = "completed";
                }
            
                _mailService.SendMailAsync(data.user.email, "Change order status", FormatEmailHtml(data));

                _context.Update(data);
                await _context.SaveChangesAsync();
                return new CustomResult()
                {
                    Status = 200,
                    Message = "Change Status Success!"
                };
            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    Message = "Server Error"
                };
            }
        }

        public async Task<CustomResult> GetAll()
        {
            try
            {
                var list = await _context.Orders.Include(e => e.user).ThenInclude(e => e.userdetails).Select(e => new OrderRes()
                {
                    id = e.id,
                    customerName = e.user.userdetails.last_name + " " + e.user.userdetails.first_name,
                    created_date = e.created_date,
                    contact_number = e.phone,
                    address = e.address,
                    total = e.total,
                    status = e.status
                }).ToListAsync();
                return new CustomResult()
                {
                    Status = 200,
                    data = list
                };
            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    Message = "Server Error!" + ex.InnerException.Message
                };
            }
        }

        public Task<CustomResult> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomResult> getByMonth(DateTime month)
        {
            try
            {
                var list = await _context.Orders.Where(e => EF.Functions.DateDiffMonth(month, e.created_date) == 0).Include(e => e.user).ThenInclude(e => e.userdetails).Select(e => new OrderRes()
                {
                    id = e.id,
                    customerName = e.user.userdetails.last_name + " " + e.user.userdetails.first_name,
                    created_date = e.created_date,
                    contact_number = e.user.userdetails.contact_number,
                    address = e.user.userdetails.address,
                    total = e.total,
                    status = e.status
                }).OrderByDescending(e => e.created_date).ToListAsync();
                return new CustomResult()
                {
                    Status = 200,
                    data = list
                };
            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    Message = "Server Error!" + ex.InnerException.Message
                };
            }
        }

        private class OrderRes
        {
            public string id { get; set; }
            public string customerName { get; set; }
            public DateTime created_date { get; set; }

            public float total { get; set; }

            public string address { get; set; }

            public string contact_number { get; set; }

            public string status { get; set; }

        }

        private string FormatEmailHtml(Order order)
        {
            string imgUrl = "<img src=\"https://firebasestorage.googleapis.com/v0/b/techwizwebapp.appspot.com/o/Images%2Fef8273f5-d9bf-4b95-8602-5f1de021201a.png?alt=media&token=c9257fe5-ce0a-46d3-b541-a1333d0c3f58\" alt=\"Image\" width=\"200\" height=\"160\" />";
            string emailHtml = $@"
<html>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #4CAF50;'>Order Confirmation</h2>
    <p>Dear {order?.user?.userdetails?.first_name} {order?.user?.userdetails?.last_name},</p>
    <p>Thank you for your order!</p>
    <p>Your order <strong># {order?.id} </strong> was placed on [orderDate] and is being processed. We will notify you when it has been shipped.</p>

    <h3>Order Summary</h3>
    <p><strong>Order Number:</strong> {order?.id}</p>
    <p><strong>Order Date:</strong> {order?.created_date}</p>          
    <p><strong>Shipping Address:</strong> {order?.address}</p>

    <h3>Order Total</h3>
    <p><strong>Total:</strong> {order?.total}</p>

    <p>If you have any questions about your order, feel free to <a href= [fronendURl] >contact us</a>.</p>
    <p>Thank you for shopping with us!</p>
    
    <p>Best regards,<br>The Decor Vista</p>
    <div>{imgUrl}</div>
</body>
</html>
";
            return emailHtml;
        }
    }
}
