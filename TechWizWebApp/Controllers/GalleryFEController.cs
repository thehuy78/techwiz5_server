using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GalleryFEController : ControllerBase
    {
        private IGalleryFE _galleryFE;
        public GalleryFEController(IGalleryFE iGalleryFE)
        {
            _galleryFE = iGalleryFE;
        }
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _galleryFE.GetAll();

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
            var result = await _galleryFE.GetById(id);

            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("GetByRoomType/{id}")]
        public async Task<ActionResult> GetByRoomType(int id)
        {
            var result = await _galleryFE.GetByRoomType(id);

            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


        [HttpGet("GetByRoomTypeUrl/{url}")]
        public async Task<ActionResult> GetByRoomTypeUrl(string url)
        {
            var result = await _galleryFE.GetByRoomTypeUrl(url);

            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


        [HttpGet("GetNewestByRoomType/{url}")]
        public async Task<ActionResult> GetNewestByRoomType(string url)
        {
            var result = await _galleryFE.GetNewest(url);

            if (result.Status == 200)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

    }
}
