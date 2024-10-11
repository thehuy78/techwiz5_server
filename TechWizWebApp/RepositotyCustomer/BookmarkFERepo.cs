using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.InterfaceCustomer;


namespace TechWizWebApp.RepositotyCustomer
{
    public class BookmarkFERepo : IBookmarkFE
    {
        private readonly DecorVistaDbContext _context;
        public BookmarkFERepo( DecorVistaDbContext context )
        {
            _context = context;
        }
        public Task<CustomResult> DeleteBookmark(BookmarkRes res)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomResult> GetBookmarkByUser(int id)
        {
            try
            {
                var rs = await _context.Subcribes.Include(e=>e.gallery).ThenInclude(e => e.room_type).Where(e=>e.user_id == id && e.gallery.status == true).ToListAsync();
                return new CustomResult()
                {
                    data = rs,
                    Status = 200,
                    Message = "get success"
                };
            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    data = ex.Message,
                    Status = 400,
                    Message = "fails"
                };
            }
        }

        public async Task<CustomResult> SaveBookmark(BookmarkRes res)
        {
            try
            {
                var rsuser = await _context.Users.SingleOrDefaultAsync(e => e.id == res.id_user);
                var rsgallery = await _context.Galleries.SingleOrDefaultAsync(e => e.id == res.id_gallery);
                var checkexists = await _context.Subcribes.FirstOrDefaultAsync(e=>e.user_id == res.id_user && e.gallery_id ==res.id_gallery);
                if(checkexists != null)
                {
                    _context.Remove(checkexists);
                    await _context.SaveChangesAsync();
                    return new CustomResult()
                    {
                        data = null,
                        Status = 200,
                        Message = "Unsubscriber"
                    };
                }
                if (rsuser == null || rsgallery == null)
                {
                    return new CustomResult()
                    {
                        data = null,
                        Status = 400,
                        Message = "User or gallery not exist"
                    };
                }
                var bookmark = new Subcribe();
                bookmark.user_id = res.id_user;
                bookmark.gallery_id = res.id_gallery;
                _context.Add(bookmark);
                await _context.SaveChangesAsync();
                return new CustomResult()
                {
                    data = null,
                    Status = 200,
                    Message = "Success"
                };
            }catch (Exception ex)
            {
                return new CustomResult()
                {
                    data = ex.Message,
                    Status = 400,
                    Message = "fails"
                };
            }
        }


      
    }

    public class BookmarkRes()
    {
      public  int id_gallery { get; set; }
      public  int id_user { get; set; }
    }

}
