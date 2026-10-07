using Application.Interfaces.Persistence;
using Domain.Entities.Resumes;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.Repository.Resumes
{
    public class ResumeRepository : IResumeRepository
    {
        private readonly AppDBcontext _context;

        public ResumeRepository(AppDBcontext context)
        {
            _context = context;
        }

        public async Task AddAsync(Resume resume)
        {
            await _context.Resumes.AddAsync(resume);
            await _context.SaveChangesAsync();
        }

        public async Task<Resume?> GetByIdAsync(Guid id)
        {
            return await _context.Resumes
                .Include(r => r.versions)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateAsync(Resume resume)
        {
            _context.Resumes.Update(resume);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Resume>> GetByCandidateIdAsync(Guid candidateId, int limit, int offset, CancellationToken cancellationToken)
        {
            return await _context.Resumes
                .Where(r => r.CandidateId == candidateId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedDate)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }

        public async Task AddVersionAsync(ResumeVersion version)
        {
            await _context.ResumeVersions.AddAsync(version);
            await _context.SaveChangesAsync();
        }

        public async Task<ResumeVersion?> GetVersionByIdAsync(Guid versionId)
        {
            return await _context.ResumeVersions.FirstOrDefaultAsync(v => v.Id == versionId);
        }

        public async Task<List<ResumeVersion>> GetVersionsByResumeIdAsync(Guid resumeId)
        {
            return await _context.ResumeVersions
                .Where(v => v.ResumeId == resumeId)
                .OrderBy(v => v.VersionNumber)
                .ToListAsync();
        }
    }
}
