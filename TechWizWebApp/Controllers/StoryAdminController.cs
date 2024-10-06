using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using TechWizWebApp.Domain;
using TechWizWebApp.Interfaces;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoryAdminController : ControllerBase
    {

        private readonly IStoryAdmin _story;

        public StoryAdminController(IStoryAdmin story)
        {
            _story = story;
        }


        [HttpPost]
        [Authorize(Roles = "designer")]
        [Route("create_new_story")]
        public async Task<IActionResult> CreateNewStory([FromForm] Story story)
        {
            var customResult = await _story.CreateNewStory(story);

            return Ok(customResult);
        }

        [HttpGet]
        [Authorize(Roles = "designer, admin")]
        [Route("get_designer_story")]
        public async Task<IActionResult> GetDesignerStory([FromQuery] int designer_id, [FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string from ="", [FromQuery] string to = "")
        {
            var customPaging = await _story.GetDesignerStories(designer_id, pageNumber, pageSize, from, to);

            return Ok(customPaging);
        }

        [HttpPatch]
        [Authorize(Roles = "designer")]
        [Route("update_designer_story")]
        public async Task<IActionResult> UpdateDesignerStory([FromForm] Story story)
        {
            var customResult = await _story.UpdateStory(story);

            return Ok(customResult);
        }
    }
}
