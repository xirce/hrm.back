namespace SkillSystem.Application.Services.SalaryRanges.Models;

public record SalaryRangeRequest
{
    public int GradeId { get; set; }
    public decimal MinimumWage { get; set; }
    public decimal MaximumWage { get; set; }
}
