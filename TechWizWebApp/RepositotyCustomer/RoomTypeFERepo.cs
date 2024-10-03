using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TechWizWebApp.Data;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.RepositotyCustomer
{
    public class RoomTypeFERepo : IRoomTypeFE
    {
        private DecorVistaDbContext _context;
        public RoomTypeFERepo(DecorVistaDbContext decorVistaDbContext)
        {
            _context = decorVistaDbContext;
        }
        public async Task<CustomResult> GetAll()
        {
            try
            {
                var list = await _context.RoomTypes.ToListAsync();
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
                    Message = "Server Error!"
                };
            }
        }
    }
}
