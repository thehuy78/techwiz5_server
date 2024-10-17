using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Data;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.RepositotyCustomer
{
    public class NotificationFERepo:INotificationFE
    {
        private DecorVistaDbContext _db;
        public NotificationFERepo(DecorVistaDbContext db)
        {
            _db = db;
        }

        public async Task<CustomResult> Deleted(int id)
        {
            try
            {
                var rs = await _db.Notifications.SingleOrDefaultAsync(e => e.id == id);
                if (rs != null)
                {
                    _db.Notifications.Remove(rs);
                    await _db.SaveChangesAsync();
                    
                }
                return new CustomResult
                {
                    Status = 200,
                    data = rs,
                    Message = "get success"
                };

            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    data = null,
                    Message = ex.Message
                };
            }
        }

        public async Task<CustomResult> GetNotificationByUser(int id)
        {
           
            try
            {
                var rs = await _db.Notifications.Where(e => e.user_id == id).OrderByDescending(e => e.created_date).ToListAsync();
                return new CustomResult
                {
                    Status = 200,
                    data = rs,
                    Message = "get success"
                };
            }
            catch (Exception ex)
            {
                return new CustomResult() 
                { 
                    Status=400,
                    data=null,
                    Message=ex.Message
                };
            }
            
        }

        public async Task<CustomResult> ReadAction(int id)
        {
            try
            {
                var rs = await _db.Notifications.SingleOrDefaultAsync(e=>e.id == id);
                if(rs != null)
                {
                    rs.is_read = true;
                    await _db.SaveChangesAsync();
                    return new CustomResult { Status = 200 };
                }
                return new CustomResult { Status = 300 };
            }
            catch (Exception ex)
            {
                return new CustomResult { Status = 400 };
            }
        }
    }
}
