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
        Task<CustomResult> fitterStatus(bool? status, string? name);


    }
}
