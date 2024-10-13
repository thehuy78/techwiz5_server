using TechWizWebApp.Data;
using TechWizWebApp.RepositotyCustomer;

namespace TechWizWebApp.InterfaceCustomer
{
    public interface IBookmarkFE
    {

        Task<CustomResult> GetBookmarkByUser(int id);
        Task<CustomResult> SaveBookmark(BookmarkRes res);
        Task<CustomResult> DeleteBookmark(BookmarkRes res);
    }
}
