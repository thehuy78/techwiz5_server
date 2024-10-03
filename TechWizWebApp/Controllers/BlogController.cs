﻿using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Utilities;
using System.Reflection.Metadata;
using System.Security.Claims;
using TechWizWebApp.Domain;
using TechWizWebApp.Interfaces;
using TechWizWebApp.RequestModels;

namespace TechWizWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin, designer")]
    public class BlogController : ControllerBase
    {
        private IBlog _blog;
        public BlogController(IBlog blog)

        {
            _blog = blog;
        }


        [HttpGet("getblogbyid")]
        public async Task<IActionResult> readBlogById(int id)
        {
            var result = await _blog.readBlogById(id);
            return Ok(result);
        }

        [HttpPost]

        public async Task<IActionResult> createBlog([FromForm] Blog blog)
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "Id").Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role).Value;

            if (roleClaim == "designer")
            {
                int userId;
                int.TryParse(idClaim, out userId);
                var result = await _blog.createBlog(userId, blog);

                return Ok(result);
            }

            if (roleClaim == "admin")
            {
                int? userId = null;
                var result = await _blog.createBlog(userId, blog);

                return Ok(result);
            }
            return Ok();

        }
        [HttpGet]

        public async Task<ActionResult> GetAllBlog()
        {
            var list = await _blog.readAll();
            return Ok(list);
        }
        [HttpPut]

        public async Task<ActionResult> UpdateBlog([FromForm] Blog blog)
        {
            var result = await _blog.updateBlog(blog);
            return Ok(result);
        }

        [HttpGet("getblogbyuserid")]
        public async Task<ActionResult> GetBlogByUserId(int? ownerBlogId)
        {
            var result = await _blog.readBlogByDesigner(ownerBlogId);
            return Ok(result);

        }

        [HttpGet("getblogbytitle")]
        public async Task<ActionResult> GetBlogByTitle(string title)
        {
            var result = await _blog.searchBlogByName(title);
            return Ok(result);

        }
        [HttpPut("activeBlog")]
        public async Task<ActionResult> ActiveBlog([FromForm] RequestActiveBlog request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            var result = await _blog.activeBlog(request);
            return Ok(result);
        }
        [HttpGet("getblogbystatus")]
        public async Task<ActionResult> GetBlogByStatus(bool? status, string? name)
        {
            var result = await _blog.fitterStatus(status, name);
            return Ok(result);
        }
    }
}
