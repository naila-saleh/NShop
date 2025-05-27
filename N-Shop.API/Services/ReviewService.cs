using N_Shop.API.Data;
using N_Shop.API.Models;
using N_Shop.API.Services.IService;

namespace N_Shop.API.Services;

public class ReviewService: Service<Review>, IReviewService
{
    private readonly ApplicationDbContext _context;
    public ReviewService(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<bool> EditAsync(int id,Review review,CancellationToken cancellationToken = default)
    {
        Review? reviewInDb = await _context.Reviews.FindAsync(id);
        if (reviewInDb == null) return false;
        review.Id=reviewInDb.Id;
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}