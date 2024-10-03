using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.Interface;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GalleryController : ControllerBase
    {
        private IGallery _gallery;
        private readonly DecorVistaDbContext _context;
        public GalleryController(IGallery gallery, DecorVistaDbContext decorVistaDbContext)
        {
            _gallery = gallery;
            _context = decorVistaDbContext;
        }


        [HttpPost("createNew")]
        public async Task<ActionResult> Create([FromForm] Gallery gallery)
        {
            var result = await _gallery.Create(gallery);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("GetAll")]

        public async Task<ActionResult> GetAll()
        {
            var result = await _gallery.GetAll();
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
        public async Task<ActionResult> ChangeStatus(int id)
        {
            var result = await _gallery.ChangeStatus(id);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var result = await _gallery.GetById(id);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromForm] Gallery e)
        {
            var result = await _gallery.Update(e);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        //  Nhan viet
        [HttpGet]
        public async Task<IActionResult> GetGalleryByProductId(int productId)
        {
            var product = await _context.Products
                .Include(p => p.galleryDetails)
                .ThenInclude(gd => gd.gallery)
                .Where(p => p.id == productId)
                .FirstOrDefaultAsync();


            if (product?.galleryDetails == null)
            {
                return Ok(new CustomResult
                {
                    data = null,
                    Message = "nothing",
                    Status = 400
                });
            }


            var products = await _context.Products
                .Include(p => p.functionality)
                .Include(p => p.galleryDetails)
                .ThenInclude(gd => gd.gallery)
                .Where(p => p.galleryDetails.Any(gd => gd.gallery_id == product.galleryDetails[0].gallery_id && gd.product_id != productId))
                .ToListAsync();


            return Ok(new CustomResult
            {
                data = products,
                Message = "OK",
                Status = 200
            });
        }

        //  Nhan viet
        [HttpPost]
        public async Task<IActionResult> CreateGallery()
        {
            Gallery gallery = new Gallery();
            gallery.gallery_name = "Summer";
            gallery.description = "Bo suu tap cho mua he";
            gallery.status = true;
            gallery.room_type_id = 1;
            gallery.color_tone = "Red";
            gallery.view_count = 5252;
            gallery.imageName = "00152aff-f675-4fa2-bb0a-cef158ea4fc7.png";

            int[] productIds = [1, 2, 3];
            List<GalleryDetails> galleriesDetails = new List<GalleryDetails>();
            foreach (int item in productIds)
            {
                GalleryDetails galleryDetails = new GalleryDetails();
                galleryDetails.product_id = item;
                galleryDetails.gallery = gallery;
                galleriesDetails.Add(galleryDetails);
            }

            _context.AddRange(galleriesDetails);
            _context.SaveChanges();
            return Ok("");
        }
    }
}
