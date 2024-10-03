using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Data;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionalityFEController : ControllerBase
    {
        private readonly DecorVistaDbContext _context;
        public FunctionalityFEController(DecorVistaDbContext decorVistaDbContext)
        {
            _context = decorVistaDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetFunctionalities()
        {
            var functionalites = await _context.Functionalities
                .Include(f => f.products)
                .Select(f => new
                {
                    f.id,
                    f.name,
                    f.productCount
                })
                .ToListAsync();

            return Ok(new CustomResult
            {
                data = functionalites,
                Message = "OK",
                Status = 200
            });
        }
    }
}
