using TechWizWebApp.Data;
using TechWizWebApp.Domain;

namespace TechWizWebApp.InterfaceCustomer
{
    public interface IBookingFE
    {
        Task<CustomResult> CreateBooking(Consultation e);


        Task<CustomResult> ListBookingByUserID(int iduser);
    }
}
