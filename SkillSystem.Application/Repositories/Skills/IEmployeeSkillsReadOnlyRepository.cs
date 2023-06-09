﻿using SkillSystem.Core.Entities;

namespace SkillSystem.Application.Repositories.Skills;

public interface IEmployeeSkillsReadOnlyRepository
{
    Task<EmployeeSkill?> FindEmployeeSkillAsync(Guid employeeId, int skillId);
    Task<EmployeeSkill> GetEmployeeSkillAsync(Guid employeeId, int skillId);
    Task<ICollection<EmployeeSkill>> FindEmployeeSkillsAsync(Guid employeeId);
    Task<ICollection<EmployeeSkill>> FindEmployeeSkillsAsync(Guid employeeId, IEnumerable<int> skillsIds);
}
