using System.ComponentModel.DataAnnotations;

namespace SkillSystem.Core.Entities;

/// <summary>
/// Диапозон зарплаты.
/// </summary>
public class SalaryRange
{
    /// <summary>
    /// GradeId, к которому привязан диапозон.
    /// </summary>
    [Key]
    public int GradeId { get; set; }

    /// <summary>
    /// Минимальный оклад.
    /// </summary>
    public decimal MinimumWage { get; set; }

    /// <summary>
    /// Максимальный оклад.
    /// </summary>
    public decimal MaximumWage { get; set; }
}
