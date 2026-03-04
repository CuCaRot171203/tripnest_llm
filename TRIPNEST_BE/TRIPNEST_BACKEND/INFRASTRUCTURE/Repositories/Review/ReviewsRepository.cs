using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Reviews;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Review
{
    public class ReviewsRepository : IReviewsRepository
    {
        private readonly TripnestDbContext _db;

        public ReviewsRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task AddReviewAsync(Reviews review, CancellationToken ct = default)
        {
            _db.Reviews.Add(review);
            await _db.SaveChangesAsync(ct);
        }

        public async Task AddPropertyPhotoAsync(Propertyphotos photo, CancellationToken ct = default)
        {
            _db.Propertyphotos.Add(photo);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<(List<Reviews> Items, int Total)> GetReviewsByPropertyAsync(long propertyId, int page, int pageSize, CancellationToken ct = default)
        {
            var query = _db.Reviews
                .Where(r => r.PropertyId == propertyId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .AsQueryable();

            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync(ct);

            return (items, total);
        }

        public async Task<Reviews?> GetReviewByIdAsync(Guid reviewId, CancellationToken ct = default)
        {
            return await _db.Reviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.ReviewId == reviewId, ct);
        }

        public async Task UpdateReviewAsync(Reviews review, CancellationToken ct = default)
        {
            _db.Reviews.Update(review);
            await _db.SaveChangesAsync(ct);
        }


        public async Task DeleteReviewAsync(Reviews review, CancellationToken ct = default)
        {
            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync(ct);
        }


        public async Task<int> CountReviewsByPropertyAsync(long propertyId, CancellationToken ct = default)
        {
            return await _db.Reviews
                .Where(r => r.PropertyId == propertyId)
                .CountAsync(ct);
        }
    }
}
