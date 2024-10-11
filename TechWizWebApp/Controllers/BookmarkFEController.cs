using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechWizWebApp.InterfaceCustomer;
using TechWizWebApp.RepositotyCustomer;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarkFEController : ControllerBase
    {
        private readonly IBookmarkFE _bookmarkfe;
        public BookmarkFEController(IBookmarkFE bookmark)
        {
            _bookmarkfe = bookmark;
        }

        [HttpPost("SaveBookmark")]
        public async Task<ActionResult> SaveBookmark(BookmarkRes b)
        {
            var rs = await _bookmarkfe.SaveBookmark(b);
           if (rs.Status == 200) 
            {
            return Ok(rs);
            }
            else
            {
                return BadRequest(rs);
            }
        }

        [HttpGet("Getall/{id}")]
        public async Task<ActionResult> GetAll(int id)
        {
            var rs = await _bookmarkfe.GetBookmarkByUser(id);
            if (rs.Status == 200)
            {
                return Ok(rs);
            }
            else
            {
                return BadRequest(rs);
            }
        }
    }
}
