using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using TechWizWebApp.InterfaceCustomer;

namespace TechWizWebApp.RepositotyCustomer
{
    public class ReviewFERepo : IReviewFE
    {
        private DecorVistaDbContext _db;
        public ReviewFERepo(DecorVistaDbContext db)
        {
            _db = db;
        }
        public async Task<CustomResult> listReviewConsultationByUser(int id)
        {
            try
            {
                var list = await _db.Reviews.Where(e => e.id_booking != null && e.user_id == id).OrderByDescending(e => e.create_at).ToListAsync();

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
                    Message = "Server Error"
                };
            }
        }

        public async Task<CustomResult> listReviewProduct(int id)
        {
            try
            {
                var list = await _db.Reviews.Include(e=>e.user).ThenInclude(ud=>ud.userdetails).Where(e => e.product_id != null && e.product_id == id).Select(e => new ReviewProduct()
                { 
                id = e.id,
                user_id = e.user_id,
                product_id = e.product_id,
                comment = e.comment,
                score = e.score,
                create_at = e.create_at,
                first_name = e.user.userdetails.first_name,
                last_name = e.user.userdetails.last_name,
                avatar = e.user.userdetails.avatar,
                }).OrderByDescending(e => e.create_at).ToListAsync();

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
                    Message = "Server Error"
                };
            }
        }

        public async Task<CustomResult> SendFeedBackConsultation(Review e)
        {
            try
            {
                e.create_at = DateTime.Now;
                _db.Reviews.Add(e);
                await _db.SaveChangesAsync();
                return new CustomResult()
                {
                    Status = 200,
                    Message = "Send Success"
                };
            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    Message = "Send Fail"
                };
            }
        }

        public async Task<CustomResult> SendFeedBackProduct(ReviewRes e)
        {
            try
            {
                var order = await _db.Orders.SingleOrDefaultAsync(a => a.id == e.orderId);
                order.status = "Finished";
                _db.Orders.Update(order);
                Review review = new Review();
                review.user_id = e.user_id;
                review.create_at = DateTime.Now;
                review.comment = e.comment;
                review.product_id = e.product_id;
                review.score = e.score;
                _db.Reviews.Add(review);
                await _db.SaveChangesAsync();
                return new CustomResult()
                {
                    Status = 200,
                    Message = "Send Success"
                };
            }
            catch (Exception ex)
            {
                return new CustomResult()
                {
                    Status = 400,
                    Message = "Send Fail"
                };
            }
        }
        public class ReviewRes
        {
            public int user_id { get; set; }

            public int? product_id { get; set; }

            public float? score { get; set; }
            public int? id_booking { get; set; }

            public DateTime? create_at { get; set; }

            public string? orderId { get; set; }

            public string? comment { get; set; }
        }


        public class ReviewProduct
        {
            public int id { get; set; }
            public int user_id { get; set; }          
            public int? product_id { get; set; }
            public float? score { get; set; }
            public DateTime? create_at { get; set; }
            public string comment { get; set; } = string.Empty;
            public string first_name { get; set; } = string.Empty;
            public string last_name { get; set; } = string.Empty;
            public string avatar { get; set; } = string.Empty;
        }
    }
}
