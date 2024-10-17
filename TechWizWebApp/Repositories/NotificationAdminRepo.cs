using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.Interfaces;
using TechWizWebApp.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TechWizWebApp.Repositories
{
    public class NotificationAdminRepo : INotificationAdmin
    {
        private readonly DecorVistaDbContext _context;
        private readonly IConfiguration _config;
        private readonly IFileService _fileService;
        private readonly IMailService _mailService;

        public NotificationAdminRepo(DecorVistaDbContext context, IConfiguration config, IFileService fileService, IMailService mailService)
        {
            _context = context;
            _config = config;
            _fileService = fileService;
            _mailService = mailService;
        }


        public async Task<CustomResult> DeleteNotification(int notificationId)
        {
            try
            {
                var notification = await _context.Notifications.SingleOrDefaultAsync(p => p.id == notificationId); 

                if(notification == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                _context.Notifications.Remove(notification);

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", null);
            }catch(Exception ex)
            {
                return new CustomResult(400,"Bad request", ex.Message);
            }
        }

        public async Task<CustomPaging> GetAdminNotification(int userId, string type, int pageNumber, int pageSize)
        {
            try
            {
                IQueryable<Notification> query;

                query = _context.Notifications;

                query = query.Where(n => n.user_id == userId);

                if(type.Length > 0)
                {
                    query = query.Where(n => n.type == type);
                }

                query = query.OrderByDescending(n => n.created_date);

                var total = query.Count();

                query = query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);


                var list = await query.ToListAsync();

                var customPaging = new CustomPaging()
                {
                    Status = 200,
                    Message = "OK",
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)total / pageSize),
                    PageSize = pageSize,
                    TotalCount = total,
                    Data = list
                };

                return customPaging;

            }
            catch(Exception ex)
            {
                return new CustomPaging()
                {
                    Status = 400,
                    Message = ex.Message,
                    CurrentPage = pageNumber,
                    TotalPages = 0,
                    PageSize = pageSize,
                    TotalCount = 0,
                    Data = null
                };
            }
        }

        public async Task<CustomResult> MarkAsReadNotification(int notificationId)
        {
            try
            {
                var notification = await _context.Notifications.SingleOrDefaultAsync(p => p.id == notificationId);

                if (notification == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                notification.is_read = true;

                _context.Notifications.Update(notification);

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", null);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<CustomResult> SendNotification(ICollection<string> sendTo, string message)
        {
            try
            {
                foreach(var to in sendTo)
                {
                    if(to == "customer")
                    {
                        var customers = await _context.Users.Include(u => u.userdetails).Where(u => u.userdetails.role == "customer").ToListAsync();

                        foreach(var customer in customers)
                        {
                            var newNotification = new Notification
                            {
                                created_date = DateTime.Now,
                                is_read = false,
                                message = message,
                                type = "customer:admin",
                                url = "/",
                                user_id = customer.id
                            };
                            _context.Notifications.Add(newNotification);
                        }
                    }
                    else
                    {
                        var designers = await _context.Users.Include(u => u.userdetails).Where(u => u.userdetails == null).ToListAsync();

                        foreach (var designer in designers)
                        {
                            var newNotification = new Notification
                            {
                                created_date = DateTime.Now,
                                is_read = false,
                                message = message,
                                type = "designer:admin",
                                url = "/",
                                user_id = designer.id
                            };
                            _context.Notifications.Add(newNotification);
                        }
                    }

                    await _context.SaveChangesAsync();

                }
                return new CustomResult(200, "Success", null);
            }
            catch(Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }
    }
}
