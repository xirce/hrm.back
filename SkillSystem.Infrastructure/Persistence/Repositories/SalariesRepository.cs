using Microsoft.EntityFrameworkCore;
using SkillSystem.Application.Common.Exceptions;
using SkillSystem.Application.Repositories.Salaries;
using SkillSystem.Core.Entities;

namespace SkillSystem.Infrastructure.Persistence.Repositories;

public class SalariesRepository : ISalariesRepository
{
    private readonly SkillSystemDbContext dbContext;

    public SalariesRepository(SkillSystemDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Salary> CreateSalaryAsync(Salary salary)
    {
        await dbContext.Salaries.AddAsync(salary);
        return salary;
    }

    public async Task<Salary?> FindSalaryByIdAsync(int salaryId)
    {
        return await dbContext.Salaries.FirstOrDefaultAsync(salary => salary.Id == salaryId);
    }

    public async Task<Salary> GetSalaryByIdAsync(int salaryId)
    {
        return await FindSalaryByIdAsync(salaryId) ?? throw new EntityNotFoundException(nameof(Salary), salaryId);
    }

    public async Task<Salary?> FindSalaryByMonthAsync(Guid employeeId, DateTime date)
    {
        return await dbContext.Salaries.OrderBy(salary => salary.StartDate)
            .LastOrDefaultAsync(salary => salary.EmployeeId == employeeId &&
            ((salary.StartDate.Month == date.Month && salary.StartDate.Year == date.Year) ||
            salary.StartDate <= date ));
    }

    public async Task<Salary> GetSalaryByMonthAsync(Guid employeeId, DateTime date)
    {
        return await FindSalaryByMonthAsync(employeeId, date) ?? throw new EntityNotFoundException(nameof(Salary), employeeId);
    }

    public async Task<IEnumerable<Salary>> GetSalariesAsync(Guid employeeId, DateTime? from, DateTime? to)
    {
        var salaries = dbContext.Salaries.Where(salary => salary.EmployeeId == employeeId)
            .Where(salary => from == null || salary.StartDate >= from ||
            (salary.StartDate.Month == from.Value.Month && salary.StartDate.Year == from.Value.Year))
            .Where(salary => to == null || salary.StartDate <= to ||
            (salary.StartDate.Month == to.Value.Month && salary.StartDate.Year == to.Value.Year));

        return await salaries.ToListAsync();
    }

    public Salary UpdateSalaryAsync(Salary salary)
    {
        dbContext.Salaries.Update(salary);
        return salary;
    }

    public async Task DeleteSalaryAsync(Salary salary)
    {
        dbContext.Salaries.Remove(salary);
        await dbContext.SaveChangesAsync();
    }
}
