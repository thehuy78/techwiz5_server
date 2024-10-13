using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.RepositotyCustomer
{
    public class BookingFERepo : IBookingFE
    {
        private DecorVistaDbContext _dbContext;
        public BookingFERepo(DecorVistaDbContext context)
        {
            _dbContext = context;
        }
        public async Task<CustomResult> CreateBooking(Consultation e)
        {
            try
            {
                _dbContext.Consultations.Add(e);
                await _dbContext.SaveChangesAsync();
                return new CustomResult()
                {
                    Status = 200,

                };
            }

            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    Message = "Server Error"
                };
            }
        }

        public async Task<CustomResult> ListBookingByUserID(int iduser)
        {
            try
            {
                var list = await _dbContext.Consultations.Include(e=>e.review).Include(e => e.interior_designer).Where(e => e.user_id == iduser).Select
                   (e => new BookingRes()
                   {
                       id = e.id,
                       scheduled_date = e.scheduled_datetime,
                       status = e.status,
                       scheduled_time = e.time,
                       first_name = e.interior_designer.first_name,
                       last_name = e.interior_designer.last_name,
                       avatar = e.interior_designer.avatar,
                       contact_number = e.interior_designer.contact_number,
                       review = e.review


                   }).ToListAsync();
                return new CustomResult()
                {
                    Status = 200,
                    data = list
                };
            }
            catch (Exception ex)
            {

                return new CustomResult()
                {
                    Status = 400,
                    data = null
                };

            }
        }
    }
}
public class BookingRes
{
    public int id { get; set; }
  

    public string status { get; set; }

    public DateTime scheduled_date { get; set; }
    public string scheduled_time { get; set; }

    public Review review { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string contact_number { get; set; }
    public string avatar { get; set; }

}