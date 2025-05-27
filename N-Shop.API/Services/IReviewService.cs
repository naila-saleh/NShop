using N_Shop.API.Models;
using N_Shop.API.Services.IService;

namespace N_Shop.API.Services;

public interface IReviewService : IService<Review>
{
    Task<bool> EditAsync(int id,Review review,CancellationToken cancellationToken = default);
}