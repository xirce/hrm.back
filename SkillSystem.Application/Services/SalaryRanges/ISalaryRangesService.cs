using SkillSystem.Application.Services.SalaryRanges.Models;

namespace SkillSystem.Application.Services.SalaryRanges;

public interface ISalaryRangesService
{
    Task<int> CreateSalaryRange(SalaryRangeRequest request);
    Task<SalaryRangeResponse> GetSalaryRangeByGrade(int gradeId);
    Task<ICollection<SalaryRangeResponse>> GetSalaryRanges();
    Task UpdateSalaryRange(int gradeId, SalaryRangeRequest request);
    Task DeleteSalaryRange(int gradeId);
}
