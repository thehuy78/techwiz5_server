using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.Services;
using TechWizWebApp.Interface;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrder _order;
        private readonly DecorVistaDbContext _context;
        private readonly IMailService _mailService;
        public OrderController(IOrder order, DecorVistaDbContext decorVistaDbContext, IMailService mailService)
        {
            _order = order;
            _context = decorVistaDbContext;
            _mailService = mailService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _order.GetAll();
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost("GetByMonth")]
        public async Task<ActionResult> GetByMonth([FromForm] OrderForm o)
        {
            var result = await _order.getByMonth(o.Month);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpGet("ChangeStatus/{id}")]
        public async Task<ActionResult> ChangeStatus(string id)
        {
            var result = await _order.ChangeStatus(id);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        public class OrderForm
        {


            public DateTime Month { get; set; }
        }



        // nhan viet 

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderRequest request)
        {
            // lay user de tao order

            User user = await _context.Users.Include(u => u.userdetails).Where(u => u.id == request.user_Id).FirstOrDefaultAsync();

            //tao order theo user
            Order order = new Order();
            order.user_id = request.user_Id;
            order.address = "6363 Le Loi,Go Vap";
            order.status = "packaged";

            //tao danh sach order details
            List<OrderDetails> orderDetails = new List<OrderDetails>();
            foreach (var item in request.variant_and_quanity_requests)
            {
                var variant = await _context.Variants.FindAsync(item.variant_Id);
                var od = new OrderDetails
                {
                    order_id = order.id,
                    variant_id = variant.id,
                    quanity = item.quanity
                };
                float total = od.quanity * variant.price;
                order.total += total;
                orderDetails.Add(od);
            }

            order.order_details = orderDetails;

            _context.Add(order);
            await _context.SaveChangesAsync();


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
            return Ok("");
        }

        [HttpGet("GetOrdersByUserId")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.user)
                .Where(o => o.user_id == userId)
                .Select(o => new
                {
                    o.id,
                    o.total,
                    o.address,
                    o.user
                })
                .ToListAsync();

            return Ok(new CustomResult
            {
                data = orders,
                Message = "oke",
                Status = 200
            });
        }
    }

    #region
    public class OrderRequest
    {
        public int user_Id { get; set; }
        public List<VariantAndQuanityRequest> variant_and_quanity_requests { get; set; }

    }
    public class VariantAndQuanityRequest
    {
        public int variant_Id { get; set; }
        public int quanity { get; set; }
    }
    #endregion

}

