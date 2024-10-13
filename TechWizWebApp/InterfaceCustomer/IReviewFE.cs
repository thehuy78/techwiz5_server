using TechWizWebApp.Data;
using TechWizWebApp.Domain;
using static TechWizWebApp.RepositotyCustomer.ReviewFERepo;

namespace TechWizWebApp.InterfaceCustomer
{
    public interface IReviewFE
    {
        Task<CustomResult> listReviewProduct(int id);

        Task<CustomResult> listReviewConsultationByUser(int id);

        Task<CustomResult> SendFeedBackProduct(ReviewRes e);

        Task<CustomResult> SendFeedBackConsultation(Review e);


        Task<CustomResult> OrderReview(int id);
    }
}
