﻿using Microsoft.EntityFrameworkCore;
using SkillSystem.Application.Common.Exceptions;
using SkillSystem.Application.Common.Extensions;
using SkillSystem.Application.Repositories.Skills;
using SkillSystem.Core.Entities;

namespace SkillSystem.Infrastructure.Persistence.Repositories;

public class SkillsRepository : ISkillsRepository
{
    private readonly SkillSystemDbContext dbContext;

    public SkillsRepository(SkillSystemDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task CreateSkillAsync(Skill skill)
    {
        await dbContext.Skills.AddAsync(skill);
    }

    public async Task<Skill?> FindSkillByIdAsync(int skillId, bool includeSubSkills = false)
    {
        var skills = dbContext.Skills.AsNoTracking();

        if (includeSubSkills)
            skills = skills.Include(skill => skill.SubSkills.OrderBy(subSkill => subSkill.Id));

        return await skills.FirstOrDefaultAsync(skill => skill.Id == skillId);
    }

    public async Task<Skill> GetSkillByIdAsync(int skillId, bool includeSubSkills = false)
    {
        return await FindSkillByIdAsync(skillId, includeSubSkills)
               ?? throw new EntityNotFoundException(nameof(Skill), skillId);
    }

    public async Task<IEnumerable<Skill>> GetSubSkillsAsync(int groupId)
    {
        var subSkills = await dbContext.Skills
            .AsNoTracking()
            .Where(skill => skill.Id == groupId)
            .Select(skill => skill.SubSkills)
            .FirstOrDefaultAsync();

        if (subSkills is null)
            throw new EntityNotFoundException(nameof(Skill), groupId);

        return subSkills;
    }

    public async Task<IEnumerable<Skill>> TraverseSkillAsync(int groupId)
    {
        var skill = await GetSkillByIdAsync(groupId);
        return skill.Traverse(subSkill => GetSubSkillsAsync(subSkill.Id).GetAwaiter().GetResult());
    }

    public async IAsyncEnumerable<Skill> GetGroups(int skillId)
    {
        var skill = await GetSkillByIdAsync(skillId);
        var currentGroupId = skill.GroupId;
        while (currentGroupId.HasValue)
        {
            var currentGroup = await GetSkillByIdAsync(currentGroupId.Value);
            yield return currentGroup;
            currentGroupId = currentGroup.GroupId;
        }
    }

    public IQueryable<Skill> FindSkills(string? title = default)
    {
        var query = dbContext.Skills.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(skill => skill.Title.Contains(title));

        return query.OrderBy(skill => skill.Id);
    }

    public void UpdateSkill(Skill skill)
    {
        dbContext.Skills.Update(skill);
    }

    public void DeleteSkill(Skill skill)
    {
        dbContext.Skills.Remove(skill);
    }
}
