using Application.Interfaces.Persistence;
using Domain.Entities.Companies;
using CompanyEntity = Domain.Entities.Companies.Company;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository.Company
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AppDBcontext _context;

        public CompanyRepository(AppDBcontext context)
        {
            _context = context;
        }

        public async Task AddAsync(CompanyEntity company)
        {
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
        }

        public async Task<CompanyEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Companies
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(CompanyEntity company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CompanyEntity>> SearchAsync(string? name, int limit, int offset, CancellationToken cancellationToken)
        {
            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(c => EF.Functions.ILike(c.Name, $"%{name}%"));

            return await query.Skip(offset).Take(limit).ToListAsync(cancellationToken);
        }
    }
}
