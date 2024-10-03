using TechWizWebApp.Controllers;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.RequestModels;

namespace TechWizWebApp.Interfaces
{
    public interface IBlog
    {
        Task<CustomResult> readBlogByDesigner(int? ownerBlogId);
        Task<CustomResult> readAll();
        Task<CustomResult> createBlog(int? user_id, Blog blog);
        Task<CustomResult> updateBlog(Blog blog);
        Task<CustomResult> activeBlog(RequestActiveBlog request);
        Task<CustomResult> searchBlogByName(string name);
        Task<CustomResult> readBlogById(int id);
        Task<CustomPaging> getBlogByAdmin(int pageNumber, int pageSize, bool status, List<string> by, string designerName, string name);
        Task<CustomResult> UpdateBlog(RequestUpdateBlog requestUpdateBlog);

        Task<CustomPaging> getBlogByDesigner(int userId, int pageNumber, int pageSize, bool status, string name);
    }
}
