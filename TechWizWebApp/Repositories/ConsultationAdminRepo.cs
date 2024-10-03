using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Cms;
using System.ComponentModel.Design;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.Interfaces;

namespace TechWizWebApp.Repositories
{
    public class ConsultationAdminRepo : IConsultationAdmin
    {
        private readonly DecorVistaDbContext _context;
        private readonly IConfiguration _config;

        public ConsultationAdminRepo(DecorVistaDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<CustomResult> AcceptConsultation(int designerId, int consultationId)
        {
            try
            {
                var consultation = await _context.Consultations.Include(c => c.interior_designer).SingleOrDefaultAsync(c => c.id == consultationId && c.interior_designer.user_id == designerId);

                if(consultation == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                consultation.status = "accepted";

                _context.Consultations.Update(consultation);

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", consultation);
            }
            catch(Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }



        public async Task<CustomResult> DenyConsultation(int designerId, int consultationId)
        {
            try
            {
                var consultation = await _context.Consultations.Include(c => c.interior_designer).SingleOrDefaultAsync(c => c.id == consultationId && c.interior_designer.user_id == designerId);

                if (consultation == null)
                {
                    return new CustomResult(404, "Not found", null);
                }

                consultation.status = "denied";

                _context.Consultations.Update(consultation);

                await _context.SaveChangesAsync();

                return new CustomResult(200, "Success", consultation);
            }
            catch (Exception ex)
            {
                return new CustomResult(400, "Bad request", ex.Message);
            }
        }

        public async Task<CustomPaging> GetAdminCustomerConsultation(string status, int pageNumber, int pageSize)
        {
            try
            {
                IQueryable<Consultation> query;

                query = _context.Consultations;

                query = _context.Consultations.Where(c => c.status == status);

                var total = query.Count();

                query = query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

                var list = await query.Include(c => c.user.userdetails).Include(c => c.interior_designer).Select(p => new
                {
                    id = p.id,
                    user_id = p.user_id,
                    customer = new
                    {
                        first_name = p.user.userdetails != null ? p.user.userdetails.first_name : p.user.interiordesigner.first_name,
                        last_name = p.user.userdetails != null ? p.user.userdetails.last_name : p.user.interiordesigner.last_name,
                        avatar = p.user.userdetails != null ? p.user.userdetails.avatar : p.user.interiordesigner.avatar,
                        contact_number = p.user.userdetails != null ? p.user.userdetails.contact_number : p.user.interiordesigner.contact_number
                    },
                    designer = p.interior_designer ,
                    address = p.address,
                    notes = p.notes,
                    scheduled_datetime = p.scheduled_datetime,
                }).ToListAsync(); 

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
            catch (Exception ex)
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


        public async Task<CustomPaging> GetCustomerConsultation(int designerId, string status, int pageNumber, int pageSize)
        {
            try
            {
                var designer = await _context.InteriorDesigners.SingleOrDefaultAsync(i => i.user_id == designerId);

                IQueryable<Consultation> query;

                query = _context.Consultations;

                query = _context.Consultations.Where(c => c.designer_id == designer.id && c.status == status);

                var total = query.Count();

                query = query.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

                var list = await query.Include(c => c.user.userdetails).Include(c => c.interior_designer).Select(p => new
                {
                    id = p.id,
                    user_id = p.user_id,
                    customer = new
                    {
                        first_name = p.user.userdetails != null ? p.user.userdetails.first_name : p.user.interiordesigner.first_name,
                        last_name = p.user.userdetails != null ? p.user.userdetails.last_name : p.user.interiordesigner.last_name,
                        avatar = p.user.userdetails != null ? p.user.userdetails.avatar : p.user.interiordesigner.avatar,
                        contact_number = p.user.userdetails != null ? p.user.userdetails.contact_number : p.user.interiordesigner.contact_number
                    },
                    address = p.address,
                    notes = p.notes,
                    scheduled_datetime = p.scheduled_datetime,
                }).ToListAsync();

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
            catch (Exception ex)
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
    }
}
