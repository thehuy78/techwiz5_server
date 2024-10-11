using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;


namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartFEController : ControllerBase
    {
        private readonly DecorVistaDbContext _context;

        public CartFEController(DecorVistaDbContext context)
        {
            _context = context;
        }

        [HttpPost("add_cart")]
        
        public async Task<IActionResult> AddToCart([FromForm] CartRequestUser cartRequest)
        {
            try
            {
                var idClaim = User.Claims.FirstOrDefault(c => c.Type == "id").Value;
                int.TryParse(idClaim, out int userId);

                var cart = await _context.Carts.SingleOrDefaultAsync(e => e.user_id == userId && e.variant_id == cartRequest.idVariant);
                if (cart == null)
                {
                    var newCart = new Cart()
                    {
                        user_id = userId,
                        variant_id = cartRequest.idVariant,
                        quanity = cartRequest.quantity,
                        product_id = cartRequest.productid,

                    };

                    _context.Carts.Add(newCart);
                    await _context.SaveChangesAsync();

                    return Ok(new CustomResult(200, "Success", newCart));
                }
                else
                {
                    cart.quanity = cart.quanity + cartRequest.quantity;
                    await _context.SaveChangesAsync();

                    return Ok(new CustomResult(200, "Success", null));
                }




            }
            catch (Exception ex)
            {
                return BadRequest(new CustomResult(400, "Bad request", ex.Message));
            }
        }

        [HttpGet("remove_cart/{cartId}")]
        public async Task<IActionResult> RemoveCart(int cartId)
        {
            try
            {
                var idClaim = User.Claims.FirstOrDefault(c => c.Type == "id").Value;
                int.TryParse(idClaim, out int userId);

                var cart = await _context.Carts.SingleOrDefaultAsync(c => c.id == cartId && c.user_id == userId);

                if (cart != null)
                {
                    _context.Carts.Remove(cart);
                }
                else
                {
                    return Ok(new CustomResult(400, "Not found", null));
                }

                await _context.SaveChangesAsync();

                return Ok(new CustomResult(200, "Success", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new CustomResult(400, "Bad request", ex.Message));
            }
        }

        [HttpGet("get_list_cart")]

        public async Task<IActionResult> GetListCart()
        {
            try
            {
                var idClaim = User.Claims.FirstOrDefault(c => c.Type == "id").Value;
                int.TryParse(idClaim, out int userId);

                var cart = await _context.Carts.Where(c => c.user_id == userId).Include(c => c.variant.variantattributes).Include(c => c.product).Select(p => new
                {
                    id = p.id,
                    name = p.product.productname,
                    attribute = p.variant.variantattributes,
                    quantity = p.quanity,
                    price = p.variant.price,
                    image = p.product.imageName,
                }).ToListAsync();

                await _context.SaveChangesAsync();

                return Ok(new CustomResult(200, "Success", cart));
            }
            catch (Exception ex)
            {
                return BadRequest(new CustomResult(400, "Bad request", ex.Message));
            }
        }





    }
    public class CartRequestUser
    {
        public int idVariant { get; set; }
        public int quantity { get; set; }
        public int productid { get; set; }
    }






}