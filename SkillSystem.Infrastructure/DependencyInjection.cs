using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillSystem.Application.Repositories;
using SkillSystem.Application.Repositories.Departments;
using SkillSystem.Application.Repositories.Duties;
using SkillSystem.Application.Repositories.Employees;
using SkillSystem.Application.Repositories.Grades;
using SkillSystem.Application.Repositories.Positions;
using SkillSystem.Application.Repositories.Projects;
using SkillSystem.Application.Repositories.Roles;
using SkillSystem.Application.Repositories.Salaries;
using SkillSystem.Application.Repositories.SalaryRanges;
using SkillSystem.Application.Repositories.SalaryTransactions;
using SkillSystem.Application.Repositories.Skills;
using SkillSystem.Application.Services.Employees;
using SkillSystem.Infrastructure.Persistence;
using SkillSystem.Infrastructure.Persistence.Repositories;
using SkillSystem.Infrastructure.Services;

namespace SkillSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SkillSystemDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString(nameof(SkillSystem))));
        services.AddScoped<SkillSystemDbInitializer>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ISkillsReadOnlyRepository, SkillsRepository>();
        services.AddScoped<ISkillsRepository, SkillsRepository>();
        services.AddScoped<IRolesRepository, RolesRepository>();
        services.AddScoped<IGradesRepository, GradesRepository>();
        services.AddScoped<IPositionsRepository, PositionsRepository>();
        services.AddScoped<IDutiesRepository, DutiesRepository>();
        services.AddScoped<ISalariesRepository, SalariesRepository>();
        services.AddScoped<ISalaryRangesRepository, SalaryRangesRepository>();
        services.AddScoped<ISalaryTransactionsRepository, SalaryTransactionsRepository>();

        services.AddScoped<IProjectsRepository, ProjectsRepository>();
        services.AddScoped<IProjectRolesRepository, ProjectRolesRepository>();
        services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();

        services.AddScoped<IEmployeeGradesReadOnlyRepository, EmployeeGradesRepository>();
        services.AddScoped<IEmployeeGradesRepository, EmployeeGradesRepository>();

        services.AddScoped<IEmployeeSkillsReadOnlyRepository, EmployeeSkillsRepository>();
        services.AddScoped<IEmployeeSkillsRepository, EmployeeSkillsRepository>();

        services.AddScoped<IEmployeesRepository, EmployeesRepository>();

        services.AddScoped<IEmployeesService, EmployeesService>();

        return services;
    }
}
