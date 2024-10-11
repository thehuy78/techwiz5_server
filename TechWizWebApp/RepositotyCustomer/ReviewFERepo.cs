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
                var list = await _db.Reviews.Include(e=>e.Consultation).ThenInclude(e=>e.interior_designer)
                    .Include(e => e.user).ThenInclude(e=>e.userdetails)
                    .Where(e => e.Consultation.designer_id == id && e.product_id == null).OrderByDescending(e => e.create_at).Select(r=> new ListReviewBookingGet
                    {
                        id = r.id,
                        comment = r.comment,
                        score = r.score,
                        create_at= r.create_at,
                        first_name = r.user.userdetails.first_name,
                        last_name = r.user.userdetails.last_name
                        

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
             
                var detail = await _db.OrderDetails.SingleOrDefaultAsync(a => a.id == e.orderdetailId);
                detail.review_status = true;
                _db.OrderDetails.Update(detail);
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


        public async Task<CustomResult> OrderReview(int id)
        {
            try
            {
                var rs = await _db.OrderDetails.Include(e => e.order).Include(e => e.variant)
                    .ThenInclude(e => e.product)
                    .Include(e => e.variant).ThenInclude(e=>e.variantattributes)
                    .Where(e => e.order.status == "completed" && e.review_status == false).Select(e=>new ListOrderReviewRes()
                    {
                        idOrder = e.order_id,
                        create_at = e.order.created_date,
                        id_orderdetail = e.id,
                        image = e.variant.product.imageName,
                        name = e.variant.product.productname,
                        user_id = e.order.user_id,
                        product_id =  e.variant.productid,
                        variants = e.variant.variantattributes,
                        
                    }).ToListAsync();
                return new CustomResult()
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
                    Message = "Server Error"
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
            public int? orderdetailId { get; set; }
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

        public class ListReviewBookingGet
        {
            public int id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public DateTime? create_at { get; set; }
            public string comment { get; set; }
            public float? score { get; set; }

            
        }


            public class ListOrderReviewRes
        {
            public string idOrder { get; set; }
            public int user_id { get; set; }
            public int? product_id { get;set; }
            public int? id_orderdetail { get; set; }
            public DateTime? create_at { get;set;}
            public string name { get; set; }
            public string image { get; set; }

            public List<VariantAttribute> variants { get; set; }
          
     
        }
    }
}
