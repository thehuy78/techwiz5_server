using Microsoft.EntityFrameworkCore;
using TechWizWebApp.Data;
using TechWizWebApp.Interface;

namespace TechWizWebApp.Repository
{
    public class TypeRoomRepo : IRoomType
    {
        private DecorVistaDbContext _dbContext;
        public TypeRoomRepo(DecorVistaDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<CustomResult> GetAll()
        {
            try { 
                var list = await _dbContext.RoomTypes.ToListAsync();
                return new CustomResult()
                {
                    Status = 200,
                    data = list
                };
            }
            catch (Exception e) {
                return new CustomResult()
                {
                    Status = 400,
                    Message = "Server error!" + e.InnerException.Message
                };
            }
        }
    }
}
