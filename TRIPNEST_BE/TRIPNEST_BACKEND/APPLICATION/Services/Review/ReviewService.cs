using APPLICATION.DTOs.Paging;
using APPLICATION.DTOs.Reviews;
using APPLICATION.Interfaces.Review;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Review
{
    public class ReviewService 
    {
        private readonly IReviewsRepository _repo;

        public ReviewService(IReviewsRepository repo)
        {
            _repo = repo;
        }

        public async Task<ReviewResponseDto> CreateReviewAsync(long propertyId, Guid userId, CreateReviewRequestDto dto, CancellationToken ct = default)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5");
            }

            var review = new Reviews
            {
                ReviewId = Guid.NewGuid(),
                PropertyId = propertyId,
                UserId = userId,
                Rating = dto.Rating,
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddReviewAsync(review, ct);

            if (dto.Photos != null && dto.Photos.Any())
            {
                foreach (var url in dto.Photos)
                {
                    var photo = new Propertyphotos
                    {
                        PhotoId = 0,
                        PropertyId = propertyId,
                        Url = url,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _repo.AddPropertyPhotoAsync(photo, ct);
                }
            }

            return new ReviewResponseDto
            {
                ReviewId = review.ReviewId,
                PropertyId = review.PropertyId,
                UserId = review.UserId,
                Rating = review.Rating,
                Title = review.Title,
                Content = review.Content,
                CreatedAt = review.CreatedAt,
                Photos = dto.Photos
            };
        }    

        public async Task<PagedResult<ReviewListItemDto>> GetReviewsByPropertyAsync(long propertyId, int page, int pageSize, CancellationToken ct = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (items, total) = await _repo.GetReviewsByPropertyAsync(propertyId, page, pageSize, ct);


            var mapped = items.Select(r => new ReviewListItemDto
            {
                ReviewId = r.ReviewId,
                UserId = r.UserId,
                UserDisplayName = r.User?.FullName, 
                Rating = r.Rating,
                Title = r.Title,
                Content = r.Content,
                CreatedAt = r.CreatedAt,
                Photos = new List<string>() 
            }).ToList();

            return new PagedResult<ReviewListItemDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = mapped
            };
        }

        public async Task<ReviewResponseDto?> UpdateReviewAsync(
            Guid reviewId, 
            Guid requestorUserId, 
            UpdateReviewRequestDto dto, 
            CancellationToken ct = default)
        {
            var review = await _repo.GetReviewByIdAsync(reviewId, ct);
            if (review == null)
            {
                return null;
            }

            if (review.UserId != requestorUserId)
            {

                throw new UnauthorizedAccessException("Only the review owner or admin can edit the review.");
            }

            review.Rating = dto.Rating;
            review.Title = dto.Title;
            review.Content = dto.Content;

            await _repo.UpdateReviewAsync(review, ct);

            if (dto.Photos != null && dto.Photos.Any())
            {
                foreach (var url in dto.Photos)
                {
                    var photo = new Propertyphotos
                    {
                        PropertyId = review.PropertyId,
                        Url = url,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _repo.AddPropertyPhotoAsync(photo, ct);
                }
            }

            return new ReviewResponseDto
            {
                ReviewId = review.ReviewId,
                PropertyId = review.PropertyId,
                UserId = review.UserId,
                Rating = review.Rating,
                Title = review.Title,
                Content = review.Content,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<bool> DeleteReviewAsync(
            Guid reviewId, 
            Guid requestorUserId, 
            CancellationToken ct = default)
        {
            var review = await _repo.GetReviewByIdAsync(reviewId, ct);
            if (review == null)
            {
                return false;
            }

            if (review.UserId != requestorUserId)
            {
                throw new UnauthorizedAccessException("Only the review owner or admin can delete the review.");
            }

            await _repo.DeleteReviewAsync(review, ct);
            return true;
        }
    }
}
