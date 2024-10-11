using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.InterfaceCustomer;
using TechWizWebApp.Services;

namespace TechWizWebApp.RepositotyCustomer
{
    public class OrderFERepo : IOrderFE
    {
        private readonly IMailService _mailService;
        private DecorVistaDbContext _dbContext;
        public OrderFERepo(DecorVistaDbContext context, IMailService mail)
        {
            _dbContext = context;
            _mailService = mail;
        }
        public async Task<CustomResult> setOrder(RequestOrder request)
        {


            #region
            try
            {
                User user = await _dbContext.Users.Include(u => u.userdetails).Where(u => u.id == request.user_Id).FirstOrDefaultAsync();

                //tao order theo user
                Order order = new Order();
                order.user_id = request.user_Id;
                order.address = request.address;
                order.fullname = request.fullname;
                order.phone = request.phone;
                order.status = "packaged";

                //tao danh sach order details
                List<OrderDetails> orderDetails = new List<OrderDetails>();
                foreach (var item in request.cartIds)
                {

                    var cart = await _dbContext.Carts.Include(e => e.variant).SingleOrDefaultAsync(e => e.id == item);
                    var od = new OrderDetails
                    {
                        order_id = order.id,
                        variant_id = cart.variant_id,
                        quanity = cart.quanity,
                    };
                    float total = od.quanity * cart.variant.price;
                    order.total += total;
                    orderDetails.Add(od);
                    _dbContext.Remove(cart);
                }

                order.order_details = orderDetails;
                order.total += 10;
                _dbContext.Add(order);

                await _dbContext.SaveChangesAsync();


                //Gui mail
                string imgUrl = "<img src=\"https://firebasestorage.googleapis.com/v0/b/techwizwebapp.appspot.com/o/Images%2Fef8273f5-d9bf-4b95-8602-5f1de021201a.png?alt=media&token=c9257fe5-ce0a-46d3-b541-a1333d0c3f58\" alt=\"Image\" />\r\n";
                string orderConfirmation = $@"
            Order Confirmation

            Dear             {user.userdetails.first_name + " " + user.userdetails.last_name}, <br/>

            We are pleased to inform you that your order (Order ID: {order.id}) has been successfully placed! <br/>

            Order Details: <br/>
            - Order Date: {order.created_date} <br/>
            - Total Amount: {order.total} <br/>

            Your order is currently being processed, and you will receive a notification once it is shipped. If you have any questions regarding your order, feel free to contact us. <br/>

            Thank you for choosing Our Company! <br/>

            Best regards, <br/>
            Our Company Customer Service Team <br/>
            <div>
            <img src=""https://firebasestorage.googleapis.com/v0/b/techwizwebapp.appspot.com/o/Images%2Fef8273f5-d9bf-4b95-8602-5f1de021201a.png?alt=media&token=c9257fe5-ce0a-46d3-b541-a1333d0c3f58"" alt=""Image"" width=""200"" height=""160"" />
            </div>
            ";


                await _mailService.SendMailAsync(user.email, "Order Information", orderConfirmation);

                return new CustomResult()
                {
                    Status = 200,
                    Message = "Success!"
                };

            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    Message = ex.Message,
                };
            }
            #endregion
        }

        public async Task<CustomResult> listOrderByUser(int id)
        {
            try
            {
                var list = await _dbContext.Orders.Include(e => e.order_details).ThenInclude(e => e.variant).ThenInclude(e => e.product).Where(e => e.user_id == id).OrderByDescending(e => e.created_date).Select(e => new OrderRes()
                {
                    id = e.id,
                    total = e.total,
                    status = e.status,
                    created_date = e.created_date,
                    details = e.order_details,


                }).ToListAsync();
                foreach (var item in list)
                {
                    int count = 0;

                    foreach (var item2 in item.details)
                    {
                        count = count + item2.quanity;
                    }
                    item.scount_item = count;
                }
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
                    Message = "Server Error"
                };
            }
        }
    }
    public class OrderRes
    {
        public string? id { get; set; }
        public float? total { get; set; }

        public string? status { get; set; }

        public List<Product> listProduct { get; set; }

        public List<OrderDetails>? details { get; set; }

        public int? scount_item { get; set; }

        public DateTime? created_date { get; set; }
    }
    public class RequestOrder
    {
        public int user_Id { get; set; }
        public string? address { get; set; }

        public string? phone { get; set; }

        public string? fullname { get; set; }
        public List<int>? cartIds { get; set; }

    }

}