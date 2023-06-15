using SkillSystem.Core.Entities;

namespace SkillSystem.Application.Repositories.SalaryRanges;

public interface ISalaryRangesRepository
{
    Task<SalaryRange> CreateSalaryRangeAsync(SalaryRange salaryRange);
    Task<SalaryRange?> FindSalaryRangeByGradeAsync(int gradeId);
    Task<SalaryRange> GetSalaryRangeByGradeAsync(int gradeId);
    Task<IEnumerable<SalaryRange>> GetSalaryRangesAsync();
    SalaryRange UpdateSalaryRangeAsync(SalaryRange salaryRange);
    Task DeleteSalaryRangeAsync(SalaryRange salaryRange);
}

