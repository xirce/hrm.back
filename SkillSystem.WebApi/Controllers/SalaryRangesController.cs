using Microsoft.AspNetCore.Mvc;
using SkillSystem.Application.Services.SalaryRanges;
using SkillSystem.Application.Services.SalaryRanges.Models;

namespace SkillSystem.WebApi.Controllers;

[Route("api/salary-ranges")]
public class SalaryRangesController : BaseController
{
    private readonly ISalaryRangesService salaryRangesService;

    public SalaryRangesController(ISalaryRangesService salaryRangesService)
    {
        this.salaryRangesService = salaryRangesService;
    }

    /// <summary>
    /// Назначить диапозон зарплат.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<int>> CreateSalaryRange(SalaryRangeRequest request)
    {
        var gradeId = await salaryRangesService.CreateSalaryRange(request);
        return Ok(gradeId);
    }

    /// <summary>
    /// Получить диапозон зарплат по gradeID.
    /// </summary>
    /// <param name="gradeId"></param>
    /// <returns></returns>
    [HttpGet("{gradeId}")]
    public async Task<ActionResult<SalaryRangeResponse>> GetSalaryRangeByGrade(int gradeId)
    {
        var salaryRange = await salaryRangesService.GetSalaryRangeByGrade(gradeId);
        return Ok(salaryRange);
    }

    /// <summary>
    /// Получить диапозоны зарплат.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SalaryRangeResponse>>> GetSalaryRanges()
    {
        var salaryRanges = await salaryRangesService.GetSalaryRanges();
        return Ok(salaryRanges);
    }

    /// <summary>
    /// Изменить диапозон зарплат с gradeID.
    /// </summary>
    /// <param name="gradeId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{gradeId}")]
    public async Task UpdateSalaryRange(int gradeId, SalaryRangeRequest request)
    {
        await salaryRangesService.UpdateSalaryRange(gradeId, request);
    }

    /// <summary>
    /// Удалить диапозон зарплат с gradeID.
    /// </summary>
    /// <param name="gradeId"></param>
    /// <returns></returns>
    [HttpDelete("{gradeId}")]
    public async Task<IActionResult> DeleteSalaryRange(int gradeId)
    {
        await salaryRangesService.DeleteSalaryRange(gradeId);
        return NoContent();
    }
}
