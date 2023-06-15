using SkillSystem.Application.Common.Models;
using SkillSystem.Application.Repositories;
using SkillSystem.Application.Repositories.Employees;
using SkillSystem.Application.Services.Employees;
using SkillSystem.Application.Services.Employees.Models;
using SkillSystem.Core.Enums;
using SkillSystem.IdentityServer4.Client;
using SkillSystem.IdentityServer4.Client.Requests;
using SkillSystem.IdentityServer4.Client.Responses;

namespace SkillSystem.Infrastructure.Services;

public class EmployeesService : IEmployeesService
{
    private readonly IUsersClient usersClient;
    private readonly IEmployeesRepository employeesRepository;
    private readonly IUnitOfWork unitOfWork;

    public EmployeesService(IUsersClient usersClient, IEmployeesRepository employeesRepository, IUnitOfWork unitOfWork)
    {
        this.usersClient = usersClient;
        this.employeesRepository = employeesRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Employee> GetOrCreateEmployeeByUserId(Guid userId)
    {
        var userResponse = await usersClient.GetUserById(userId);
        var employee = await employeesRepository.GetOrCreateEmployee(
            userResponse.Id, new Core.Entities.Employee { Id = userResponse.Id, Type = EmployeeType.Default });

        await unitOfWork.SaveChangesAsync();

        return MergeUserWithEmployee(userResponse, employee);
    }

    public async Task<PaginatedList<Employee>> SearchEmployees(SearchEmployeesRequest request)
    {
        var searchUsersRequest = new SearchUsersRequest
        {
            Query = request.Query,
            Offset = request.Offset,
            Count = request.Count
        };
        var searchUsersResponse = await usersClient.SearchUsers(searchUsersRequest);
        var usersIds = searchUsersResponse.Items
            .Select(user => user.Id)
            .ToArray();
        var existingEmployees = await employeesRepository.BatchGetEmployees(usersIds);
        var notExistingEmployees = usersIds
            .Except(existingEmployees.Select(employee => employee.Id))
            .Select(
                userId => new Core.Entities.Employee
                {
                    Id = userId,
                    Type = EmployeeType.Default
                })
            .ToArray();
        await employeesRepository.CreateEmployees(notExistingEmployees);

        await unitOfWork.SaveChangesAsync();

        var employees = JoinUsersWithEmployees(searchUsersResponse.Items, existingEmployees.Union(notExistingEmployees))
            .ToArray();

        return new PaginatedList<Employee>(
            employees, searchUsersResponse.Pagination.Offset, searchUsersResponse.Pagination.TotalCount);
    }

    public async Task<ICollection<Employee>> GetSubordinates(Guid managerId)
    {
        var userResponse = await usersClient.GetUserById(managerId);
        var manager = await employeesRepository.GetOrCreateEmployee(
            userResponse.Id, new Core.Entities.Employee { Id = userResponse.Id, Type = EmployeeType.Default });

        await unitOfWork.SaveChangesAsync();

        var subordinates = await employeesRepository.GetSubordinates(manager.Id);
        var subordinatesIds = subordinates
            .Select(subordinate => subordinate.Id)
            .ToArray();
        var getSubordinateUsersRequest = new BatchGetUsersRequest(subordinatesIds);
        var getSubordinateUsersResponse = await usersClient.BatchGetUsers(getSubordinateUsersRequest);

        return JoinUsersWithEmployees(getSubordinateUsersResponse.Users, subordinates)
            .ToArray();
    }

    private static Employee MergeUserWithEmployee(User user, Core.Entities.Employee employee)
    {
        return new Employee
        {
            Id = employee.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Patronymic = user.Patronymic,
            Type = employee.Type,
            ManagerId = employee.ManagerId
        };
    }

    private static IEnumerable<Employee> JoinUsersWithEmployees(
        IEnumerable<User> users,
        IEnumerable<Core.Entities.Employee> employees)
    {
        return users.Join(employees, user => user.Id, employee => employee.Id, MergeUserWithEmployee);
    }
}
