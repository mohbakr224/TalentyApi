using Application.Interfaces.Persistence;
using Domain.Entities.Jobs;
using JobEntity = Domain.Entities.Jobs.Job;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository.Job
{
    public class JobRepository : IJobRepository
    {
        private readonly AppDBcontext _context;

        public JobRepository(AppDBcontext context)
        {
            _context = context;
        }

        public async Task AddAsync(JobEntity job)
        {
            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();
        }

        public async Task<JobEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Jobs
                .Include(j => j.Skills)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task UpdateAsync(JobEntity job)
        {
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();
        }

        public async Task<List<JobEntity>> SearchAsync(string? title, string? location, int limit, int offset, CancellationToken cancellationToken)
        {
            var query = _context.Jobs.AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(j => EF.Functions.ILike(j.Title, $"%{title}%"));

            if (!string.IsNullOrEmpty(location))
                query = query.Where(j => EF.Functions.ILike(j.Location ?? string.Empty, $"%{location}%"));

            return await query.Skip(offset).Take(limit).ToListAsync(cancellationToken);
        }

        public async Task<List<JobEntity>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken)
        {
            return await _context.Jobs
                .Where(j => j.CompanyId == companyId && !j.IsDeleted)
                .OrderByDescending(j => j.CreatedDate)
                .ToListAsync(cancellationToken);
        }
    }
}
