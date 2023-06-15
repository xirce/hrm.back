using Microsoft.Extensions.DependencyInjection;
using SkillSystem.Application.Services.Departments;
using SkillSystem.Application.Services.Duties;
using SkillSystem.Application.Services.Employees;
using SkillSystem.Application.Services.Employees.Manager;
using SkillSystem.Application.Services.Grades;
using SkillSystem.Application.Services.Grading;
using SkillSystem.Application.Services.Grading.Grades;
using SkillSystem.Application.Services.Grading.Skills;
using SkillSystem.Application.Services.Positions;
using SkillSystem.Application.Services.Projects;
using SkillSystem.Application.Services.Roles;
using SkillSystem.Application.Services.Salaries;
using SkillSystem.Application.Services.SalariesTransactions;
using SkillSystem.Application.Services.SalaryRanges;
using SkillSystem.Application.Services.Skills;

namespace SkillSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator();

        services.AddScoped<ISkillsService, SkillsService>();
        services.AddScoped<IRolesService, RolesService>();
        services.AddScoped<IGradesService, GradesService>();
        services.AddScoped<IPositionsService, PositionsService>();
        services.AddScoped<IDutiesService, DutiesService>();
        services.AddScoped<ISalariesService, SalariesService>();
        services.AddScoped<ISalaryRangesService, SalaryRangesService>();
        services.AddScoped<ISalariesTransactionsService, SalariesTransactionsService>();


        services.AddScoped<IProjectsService, ProjectsService>();
        services.AddScoped<IProjectRolesService, ProjectRolesService>();
        services.AddScoped<IDepartmentsService, DepartmentsService>();

        services.AddScoped<IEmployeeGradesProvider, EmployeeGradesProvider>();
        services.AddScoped<IEmployeeSkillsProvider, EmployeeSkillsProvider>();

        services.AddScoped<IEmployeeGradesService, EmployeeGradesService>();
        services.AddScoped<IEmployeeGradingStrategy, AutoChangeLowerGradesStrategy>();

        services.AddScoped<IEmployeeSkillsService, EmployeeSkillsService>();
        services.AddScoped<IEmployeeSkillsChangeStrategy, AutoChangeSubSkillsStrategy>();

        services.AddScoped<IEmployeeGradingManager, EmployeeGradingManager>();

        services.AddScoped<IManagerService, ManagerService>();
        services.AddScoped<IEmployeesManager, EmployeesManager>();

        return services;
    }
}
