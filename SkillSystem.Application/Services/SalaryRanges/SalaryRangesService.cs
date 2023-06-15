using Mapster;
using SkillSystem.Application.Repositories;
using SkillSystem.Application.Repositories.SalaryRanges;
using SkillSystem.Application.Services.SalaryRanges.Models;
using SkillSystem.Core.Entities;

namespace SkillSystem.Application.Services.SalaryRanges;

public class SalaryRangesService : ISalaryRangesService
{
    private readonly ISalaryRangesRepository salaryRangesRepository;
    private readonly IUnitOfWork unitOfWork;

    public SalaryRangesService(ISalaryRangesRepository salaryRangesRepository, IUnitOfWork unitOfWork)
    {
        this.salaryRangesRepository = salaryRangesRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<int> CreateSalaryRange(SalaryRangeRequest request)
    {
        var salaryRange = request.Adapt<SalaryRange>();

        await salaryRangesRepository.CreateSalaryRangeAsync(salaryRange);
        await unitOfWork.SaveChangesAsync();

        return salaryRange.GradeId;
    }

    public async Task<SalaryRangeResponse> GetSalaryRangeByGrade(int gradeId)
    {
        var salaryRange = await salaryRangesRepository.GetSalaryRangeByGradeAsync(gradeId);
        return salaryRange.Adapt<SalaryRangeResponse>();
    }

    public async Task<ICollection<SalaryRangeResponse>> GetSalaryRanges()
    {
        var salaryRanges = await salaryRangesRepository.GetSalaryRangesAsync();
        return salaryRanges.Adapt<ICollection<SalaryRangeResponse>>();
    }

    public async Task UpdateSalaryRange(int gradeId, SalaryRangeRequest request)
    {
        var salaryRange = await salaryRangesRepository.GetSalaryRangeByGradeAsync(gradeId);
        request.Adapt(salaryRange);
        salaryRangesRepository.UpdateSalaryRangeAsync(salaryRange);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteSalaryRange(int gradeId)
    {
        var salaryRange = await salaryRangesRepository.GetSalaryRangeByGradeAsync(gradeId);
        await salaryRangesRepository.DeleteSalaryRangeAsync(salaryRange);
    }
}
