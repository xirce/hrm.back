using Mapster;
using SkillSystem.Application.Common.Extensions;
using SkillSystem.Application.Common.Services;
using SkillSystem.Application.Repositories;
using SkillSystem.Application.Repositories.Grades;
using SkillSystem.Application.Repositories.Salaries;
using SkillSystem.Application.Repositories.SalaryRanges;
using SkillSystem.Application.Repositories.SalaryTransactions;
using SkillSystem.Application.Services.Salaries.Models;
using SkillSystem.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace SkillSystem.Application.Services.SalariesTransactions;

public class SalariesTransactionsService : ISalariesTransactionsService
{
    private readonly ISalariesRepository salariesRepository;
    private readonly ISalaryTransactionsRepository transactionsRepository;
    private readonly IEmployeeGradesRepository employeeGradesRepository;
    private readonly ISalaryRangesRepository salaryRangesRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly ICurrentUserProvider currentUserProvider;

    public SalariesTransactionsService(ISalariesRepository salariesRepository,
        ISalaryTransactionsRepository transactionsRepository, IEmployeeGradesRepository employeeGradesRepository,
        ISalaryRangesRepository salaryRangesRepository,
        IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
    {
        this.salariesRepository = salariesRepository;
        this.transactionsRepository = transactionsRepository;
        this.employeeGradesRepository = employeeGradesRepository;
        this.salaryRangesRepository = salaryRangesRepository;
        this.unitOfWork = unitOfWork;
        this.currentUserProvider = currentUserProvider;
    }

    public async Task<int> SaveSalary(SalaryRequest request)
    {
        var currentUser = currentUserProvider.User;
        var userId = currentUser.GetUserId();
        if (!userId.HasValue)
        throw new NullReferenceException();
        Guid changedBy = userId.Value;
        if (!await CheckSalaryInRange(request.EmployeeId, request.Wage))
            throw new ValidationException($"The new salary is not in the range: {request.Wage}");
        var salary = await SaveSalaryAsync(request);
        await SaveTransactionAsync(salary, changedBy);
        await unitOfWork.SaveChangesAsync();
        return salary.Id;
    }

    private async Task<bool> CheckSalaryInRange(Guid employeeId, decimal wage)
    {
        var grades = await employeeGradesRepository.FindEmployeeGrades(employeeId);
        bool salaryInRange = true;
        if (grades != null && grades.Count > 0)
        {
            var grade = grades.First();
            var salaryRange = await salaryRangesRepository.GetSalaryRangeByGradeAsync(grade.GradeId);
            if (salaryRange != null)
                if (salaryRange.MinimumWage > wage || salaryRange.MaximumWage < wage)
                    salaryInRange = false;
        }
        return salaryInRange;
    }

    private async Task<Salary> SaveSalaryAsync(SalaryRequest request)
    {
        var newSalary = request.Adapt<Salary>();
        var lastSalary = await salariesRepository.FindSalaryByMonthAsync(newSalary.EmployeeId,
            newSalary.StartDate);
        var currentSalary = await salariesRepository.FindSalaryByMonthAsync(newSalary.EmployeeId, DateTime.UtcNow);
        if (currentSalary == null && (newSalary.StartDate.Month == DateTime.UtcNow.Month &&
            newSalary.StartDate.Year == DateTime.UtcNow.Year))
            return await salariesRepository.CreateSalaryAsync(newSalary);
        if (newSalary.StartDate < DateTime.UtcNow || (newSalary.StartDate.Month == DateTime.UtcNow.Month &&
            newSalary.StartDate.Year == DateTime.UtcNow.Year))
            throw new ValidationException($"Access is denied to save a salary with a date {newSalary.StartDate}");
        if (lastSalary != null && lastSalary.StartDate.Month == newSalary.StartDate.Month
            && lastSalary.StartDate.Year == newSalary.StartDate.Year)
        {
            lastSalary.Wage = newSalary.Wage;
            lastSalary.Rate = newSalary.Rate;
            lastSalary.Bonus = newSalary.Bonus;
            lastSalary.StartDate = newSalary.StartDate;
            return salariesRepository.UpdateSalaryAsync(lastSalary);
        }
        else
            return await salariesRepository.CreateSalaryAsync(newSalary);
    }

    private async Task SaveTransactionAsync(Salary salary, Guid changedBy)
    {
        var transaction = new SalaryTransaction
        {
            EmployeeId = salary.EmployeeId,
            ChangedBy = changedBy,
            SalaryChangeDate = DateTime.UtcNow,
            Wage = salary.Wage,
            Bonus = salary.Bonus,
            Rate = salary.Rate,
            StartDate = salary.StartDate
        };
        await transactionsRepository.CreateTransactionAsync(transaction);
    }
}
