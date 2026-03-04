using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Reviews
{
    public interface IReviewsRepository
    {
        Task AddReviewAsync(DOMAIN.Models.Reviews review, CancellationToken ct = default);
        Task AddPropertyPhotoAsync(Propertyphotos photo, CancellationToken ct = default);


        Task<(List<DOMAIN.Models.Reviews> Items, int Total)> GetReviewsByPropertyAsync(long propertyId, int page, int pageSize, CancellationToken ct = default);
        Task<DOMAIN.Models.Reviews?> GetReviewByIdAsync(Guid reviewId, CancellationToken ct = default);
        Task UpdateReviewAsync(DOMAIN.Models.Reviews review, CancellationToken ct = default);
        Task DeleteReviewAsync(DOMAIN.Models.Reviews review, CancellationToken ct = default);
        Task<int> CountReviewsByPropertyAsync(long propertyId, CancellationToken ct = default);
    }
}
