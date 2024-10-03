using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderAdminController : ControllerBase
    {
        private readonly DecorVistaDbContext _context;

        public OrderAdminController(DecorVistaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("get_user_order_history")]
        [Authorize(Roles ="admin, designer")]
        public async Task<IActionResult> GetUserOderHistor([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] int userId)
        {
            try
            {
                IQueryable<Order> query;

                query = _context.Orders;

                query = query.Where(o => o.user_id == userId);

                var total = query.Count();

                query = query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);


                var list = await query.Include(p => p.user.userdetails).ToListAsync();

                var customPaging = new CustomPaging()
                {
                    Status = 200,
                    Message = "OK",
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)total / pageSize),
                    PageSize = pageSize,
                    TotalCount = total,
                    Data = list
                };

                return Ok(customPaging);
            }
            catch(Exception ex)
            {
                return Ok(new CustomPaging()
                {
                    Status = 400,
                    Message = ex.Message,
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    PageSize = pageSize,
                    TotalCount = 0,
                    Data = null
                });
            }
        }
    }

}
