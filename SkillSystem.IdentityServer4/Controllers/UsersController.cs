using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSystem.IdentityServer4.Configuration;
using SkillSystem.IdentityServer4.Data.Entities;
using SkillSystem.IdentityServer4.Models.Common;
using SkillSystem.IdentityServer4.Models.Users;

namespace SkillSystem.IdentityServer4.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly UsersApiSettings usersApiSettings;

    public UsersController(UserManager<ApplicationUser> userManager, UsersApiSettings usersApiSettings)
    {
        this.userManager = userManager;
        this.usersApiSettings = usersApiSettings;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserModel>> GetUser(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return NotFound();

        var userModel = UserModel.FromApplicationUser(user);
        return Ok(userModel);
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApplicationUser>> FindUsersByQuery(
        [FromQuery] string? query = null,
        [FromQuery] int offset = 0,
        [FromQuery] int count = 100)
    {
        var usersQuery = QueryUsers(query, offset, count);

        var totalCount = await usersQuery.CountAsync();
        var users = await usersQuery.ToListAsync();

        var paginatedResponse = new PaginatedResponse<UserModel>
        {
            Items = users,
            Pagination = new PaginationResponse { Offset = offset, Count = users.Count, TotalCount = totalCount }
        };
        return Ok(paginatedResponse);
    }

    [HttpPost]
    public async Task<ActionResult<BatchGetUsersResponse>> BatchGetUsers(BatchGetUsersRequest request)
    {
        var userIds = request.UserIds
            .Select(userId => userId.ToString())
            .ToArray();
        var users = await userManager.Users
            .Where(user => userIds.Contains(user.Id))
            .Select(ProjectToUserModel())
            .ToListAsync();
        return Ok(new BatchGetUsersResponse(users));
    }

    private IQueryable<UserModel> QueryUsers(string? query, int offset, int count)
    {
        var lowerQuery = query?.ToLower();

        var usersQuery = userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(lowerQuery))
            usersQuery = usersQuery.Where(
                    user => EF.Functions.TrigramsSimilarity(user.FullName, lowerQuery)
                            >= usersApiSettings.MinSimilarity)
                .OrderByDescending(user => EF.Functions.TrigramsSimilarity(user.FullName, lowerQuery));
        else
            usersQuery = usersQuery.OrderBy(user => user.FullName);

        return usersQuery
            .Skip(offset)
            .Take(count)
            .Select(ProjectToUserModel());
    }

    private static Expression<Func<ApplicationUser, UserModel>> ProjectToUserModel()
    {
        return applicationUser => new UserModel
        {
            Id = applicationUser.Id,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            Patronymic = applicationUser.Patronymic
        };
    }
}
