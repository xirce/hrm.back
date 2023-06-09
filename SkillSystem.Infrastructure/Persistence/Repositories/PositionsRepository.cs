﻿using Microsoft.EntityFrameworkCore;
using SkillSystem.Application.Common.Exceptions;
using SkillSystem.Application.Repositories.Positions;
using SkillSystem.Application.Repositories.Positions.Filters;
using SkillSystem.Core.Entities;

namespace SkillSystem.Infrastructure.Persistence.Repositories;

public class PositionsRepository : IPositionsRepository
{
    private readonly SkillSystemDbContext dbContext;

    public PositionsRepository(SkillSystemDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task CreatePositionAsync(Position position)
    {
        await dbContext.Positions.AddAsync(position);
    }

    public async Task<Position?> FindPositionByIdAsync(int positionId)
    {
        return await dbContext.Positions.FirstOrDefaultAsync(position => position.Id == positionId);
    }

    public async Task<Position> GetPositionByIdAsync(int positionId)
    {
        return await FindPositionByIdAsync(positionId)
               ?? throw new EntityNotFoundException(nameof(Position), positionId);
    }

    public IQueryable<Position> FindPositionsAsync(PositionFilter? filter = default)
    {
        var positions = dbContext.Positions.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter?.Title))
            positions = positions.Where(role => role.Title.Contains(filter.Title));

        return positions.OrderBy(role => role.Id);
    }

    public async Task<ICollection<Duty>> GetPositionDutiesAsync(int positionId)
    {
        var positionDuties = await dbContext.Positions
            .AsNoTracking()
            .Include(position => position.Duties.OrderBy(duty => duty.Id))
            .Where(position => position.Id == positionId)
            .Select(position => position.Duties)
            .FirstOrDefaultAsync();

        if (positionDuties is null)
            throw new EntityNotFoundException(nameof(Position), positionId);

        return positionDuties;
    }

    public void UpdatePosition(Position position)
    {
        dbContext.Positions.Update(position);
    }

    public async Task AddPositionDutyAsync(int positionId, Duty duty)
    {
        var position = await GetPositionByIdAsync(positionId);

        var positionDuty = new PositionDuty(position.Id, duty.Id);

        await dbContext.PositionDuties.AddAsync(positionDuty);
    }

    public async Task DeletePositionDutyAsync(int positionId, int dutyId)
    {
        var positionDuty = await GetPositionDutyAsync(positionId, dutyId);
        dbContext.PositionDuties.Remove(positionDuty);
    }

    public void DeletePosition(Position position)
    {
        dbContext.Positions.Remove(position);
    }

    private async Task<PositionDuty> GetPositionDutyAsync(int positionId, int dutyId)
    {
        var positionDuty = await dbContext.PositionDuties
            .AsNoTracking()
            .FirstOrDefaultAsync(
                positionGrade => positionGrade.PositionId == positionId && positionGrade.DutyId == dutyId);

        if (positionDuty is null)
            throw new EntityNotFoundException(nameof(PositionDuty), new { positionId, dutyId });

        return positionDuty;
    }
}
