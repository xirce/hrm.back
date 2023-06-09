﻿namespace SkillSystem.Core.Entities;

/// <summary>
/// Должность, представляет фиксированный набор обязанностей сотрудника в проекте.
/// </summary>
public class Position : BaseEntity
{
    /// <summary>
    /// Название должности.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Набор обязанностей, которые сотрудник должен выполнять, занимая данную должность.
    /// </summary>
    public ICollection<Duty> Duties { get; set; }

    public ICollection<PositionDuty> PositionDuties { get; set; }

    /// <summary>
    /// Набор грейдов, начиная с которых можно занять данную должность.
    /// </summary>
    public ICollection<Grade> MinGrades { get; set; }

    public ICollection<PositionGrade> PositionGrades { get; set; }
}
