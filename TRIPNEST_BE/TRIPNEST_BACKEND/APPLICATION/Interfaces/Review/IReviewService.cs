using APPLICATION.DTOs.Reviews;
using INFRASTRUCTURE.Interfaces.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Review
{
    public interface IReviewService
    {
        Task<ReviewResponseDto> CreateReviewAsync(long propertyId, Guid userId, CreateReviewRequestDto dto, CancellationToken ct = default);
        Task<PagedResult<ReviewListItemDto>> GetReviewsByPropertyAsync(long propertyId, int page, int pageSize, CancellationToken ct = default);
        Task<ReviewResponseDto?> UpdateReviewAsync(Guid reviewId, Guid requestorUserId, UpdateReviewRequestDto dto, CancellationToken ct = default);
        Task<bool> DeleteReviewAsync(Guid reviewId, Guid requestorUserId, CancellationToken ct = default);
    }
}
