using SkillSystem.Application.Repositories.SalaryRanges;
using SkillSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using SkillSystem.Application.Common.Exceptions;


namespace SkillSystem.Infrastructure.Persistence.Repositories;

public class SalaryRangesRepository : ISalaryRangesRepository
{
    private readonly SkillSystemDbContext dbContext;

    public SalaryRangesRepository(SkillSystemDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<SalaryRange> CreateSalaryRangeAsync(SalaryRange salaryRange)
    {
        await dbContext.SalaryRanges.AddAsync(salaryRange);
        return salaryRange;
    }

    public async Task<SalaryRange?> FindSalaryRangeByGradeAsync(int gradeId)
    {
        return await dbContext.SalaryRanges.FirstOrDefaultAsync(salaryRange => salaryRange.GradeId == gradeId);
    }

    public async Task<SalaryRange> GetSalaryRangeByGradeAsync(int gradeId)
    {
        return await FindSalaryRangeByGradeAsync(gradeId) ?? throw new EntityNotFoundException(nameof(SalaryRange), gradeId);
    }

    public async Task<IEnumerable<SalaryRange>> GetSalaryRangesAsync()
    {
        return await dbContext.SalaryRanges.ToListAsync();
    }

    public SalaryRange UpdateSalaryRangeAsync(SalaryRange salaryRange)
    {
        dbContext.SalaryRanges.Update(salaryRange);
        return salaryRange;
    }

    public async Task DeleteSalaryRangeAsync(SalaryRange salaryRange)
    {
        dbContext.SalaryRanges.Remove(salaryRange);
        await dbContext.SaveChangesAsync();
    }
}
