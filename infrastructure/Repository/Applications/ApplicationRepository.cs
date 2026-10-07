using Application.Interfaces.Persistence;
using Domain.Entities.Applications;
using ApplicationEntity = Domain.Entities.Applications.Application;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository.Applications
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly AppDBcontext _context;

        public ApplicationRepository(AppDBcontext context)
        {
            _context = context;
        }

        public async Task AddAsync(ApplicationEntity application)
        {
            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();
        }

        public async Task<ApplicationEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Applications
                .Include(a => a.StatusHistory)
                .Include(a => a.Candidate).ThenInclude(c => c.User)
                .Include(a => a.Job).ThenInclude(j => j.Company)
                .Include(a => a.Resume)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<ApplicationEntity> UpdateAsync(ApplicationEntity application)
        {
            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
            return application;
        }

        Task IApplicationRepository.UpdateAsync(ApplicationEntity application) => UpdateAsync(application);

        public async Task<List<ApplicationEntity>> GetByCandidateIdAsync(Guid candidateId, CancellationToken cancellationToken)
        {
            return await _context.Applications
                .Include(a => a.Job).ThenInclude(j => j.Company)
                .Include(a => a.Resume)
                .Include(a => a.StatusHistory)
                .Where(a => a.CandidateId == candidateId)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ApplicationEntity>> GetByJobIdAsync(Guid jobId, string? status, int limit, int offset, CancellationToken cancellationToken)
        {
            var query = _context.Applications
                .Include(a => a.Candidate).ThenInclude(c => c.User)
                .Include(a => a.Job).ThenInclude(j => j.Company)
                .Include(a => a.Resume)
                .Include(a => a.StatusHistory)
                .Where(a => a.JobId == jobId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            return await query
                .OrderByDescending(a => a.CreatedDate)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ApplicationEntity>> GetByCompanyIdAsync(Guid companyId, string? status, int limit, int offset, CancellationToken cancellationToken)
        {
            var query = _context.Applications
                .Include(a => a.Candidate).ThenInclude(c => c.User)
                .Include(a => a.Job).ThenInclude(j => j.Company)
                .Include(a => a.Resume)
                .Include(a => a.StatusHistory)
                .Where(a => a.Job.CompanyId == companyId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            return await query
                .OrderByDescending(a => a.CreatedDate)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }
    }
}
