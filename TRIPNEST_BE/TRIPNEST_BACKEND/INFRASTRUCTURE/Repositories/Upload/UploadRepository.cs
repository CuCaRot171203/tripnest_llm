using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Upload;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Upload
{
    public class UploadRepository : IUploadRepository
    {
        private readonly TripnestDbContext _db;

        public UploadRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task<Propertyphotos> AddPhotoAsync(
            Propertyphotos photo, 
            CancellationToken ct = default)
        {
            var entry = await _db.Propertyphotos.AddAsync(photo, ct);
            return entry.Entity;
        }

        public async Task<Propertyphotos?> GetPhotoByIdAsync(
            long photoId, 
            CancellationToken ct = default)
        {
            return await _db.Propertyphotos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PhotoId == photoId, ct);
        }

        public async Task DeletePhotoAsync(
            Propertyphotos photo, 
            CancellationToken ct = default)
        {
            _db.Propertyphotos.Remove(photo);
            await Task.CompletedTask;
        }

        public async Task<Messages> AddMessageAsync(
            Messages message, 
            CancellationToken ct = default)
        {
            var entry = await _db.Messages.AddAsync(message, ct);
            return entry.Entity;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _db.SaveChangesAsync(ct);
        }
    }
}
