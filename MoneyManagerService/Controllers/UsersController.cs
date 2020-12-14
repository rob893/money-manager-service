using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using MoneyManagerService.Data.Repositories;
using MoneyManagerService.Models.QueryParameters;
using MoneyManagerService.Models.DTOs;
using MoneyManagerService.Models.Responses;
using MoneyManagerService.Entities;
using MoneyManagerService.Constants;
using Microsoft.AspNetCore.JsonPatch;
using MoneyManagerService.Extensions;
using MoneyManagerService.Models.DTOs.Budget;

namespace MoneyManagerService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ServiceControllerBase
    {
        private readonly UserRepository userRepository;
        private readonly BudgetRepository budgetRepository;
        private readonly IMapper mapper;


        public UsersController(UserRepository userRepository, BudgetRepository budgetRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.budgetRepository = budgetRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CursorPaginatedResponse<UserDto>>> GetUsersAsync([FromQuery] CursorPaginationParameters searchParams)
        {
            var users = await userRepository.SearchAsync(searchParams);
            var paginatedResponse = CursorPaginatedResponse<UserDto>.CreateFrom(users, mapper.Map<IEnumerable<UserDto>>, searchParams);

            return Ok(paginatedResponse);
        }

        [HttpGet("{id}", Name = "GetUserAsync")]
        public async Task<ActionResult<UserDto>> GetUserAsync(int id)
        {
            var user = await userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound($"User with id {id} does not exist.");
            }

            var userToReturn = mapper.Map<UserDto>(user);

            return Ok(userToReturn);
        }

        [HttpGet("{userId}/budgets")]
        public async Task<ActionResult<CursorPaginatedResponse<BudgetDto>>> GetBudgetsForUserAsync([FromRoute] int userId, [FromQuery] CursorPaginationParameters searchParams)
        {
            if (!IsUserAuthorizedForResource(userId))
            {
                return Unauthorized("You can only access your own budgets.");
            }

            var budgetParams = mapper.Map<BudgetQueryParameters>(searchParams);
            budgetParams.UserIds.Add(userId);

            var budgets = await budgetRepository.SearchAsync(budgetParams);

            var paginatedResponse = CursorPaginatedResponse<BudgetDto>.CreateFrom(budgets, mapper.Map<IEnumerable<BudgetDto>>, budgetParams);

            return Ok(paginatedResponse);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUserAsync(int id, [FromBody] JsonPatchDocument<UpdateUserDto> dtoPatchDoc)
        {
            if (dtoPatchDoc == null || dtoPatchDoc.Operations.Count == 0)
            {
                return BadRequest("A JSON patch document with at least 1 operation is required.");
            }

            var user = await userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound($"No user with Id {id} found.");
            }

            if (!User.TryGetUserId(out var userId))
            {
                return Unauthorized("You cannot do this.");
            }

            if (!User.IsAdmin() && userId != user.Id)
            {
                return Unauthorized("You cannot do this.");
            }

            if (!dtoPatchDoc.IsValid(out var errors))
            {
                return BadRequest(errors);
            }

            var patchDoc = mapper.Map<JsonPatchDocument<User>>(dtoPatchDoc);

            patchDoc.ApplyTo(user);

            await userRepository.SaveAllAsync();

            var userToReturn = mapper.Map<UserDto>(user);

            return Ok(userToReturn);
        }

        [HttpGet("roles")]
        public async Task<ActionResult<CursorPaginatedResponse<RoleDto>>> GetRolesAsync([FromQuery] CursorPaginationParameters searchParams)
        {
            var roles = await userRepository.GetRolesAsync(searchParams);
            var paginatedResponse = CursorPaginatedResponse<RoleDto>.CreateFrom(roles, mapper.Map<IEnumerable<RoleDto>>, searchParams);

            return Ok(paginatedResponse);
        }

        [Authorize(Policy = AuthorizationPolicyName.RequireAdminRole)]
        [HttpPost("{id}/roles")]
        public async Task<ActionResult<UserDto>> AddRolesAsync(int id, [FromBody] EditRoleDto roleEditDto)
        {
            if (roleEditDto.RoleNames == null || roleEditDto.RoleNames.Length == 0)
            {
                return BadRequest("At least one role must be specified.");
            }

            var user = await userRepository.GetByIdAsync(id);
            var roles = await userRepository.GetRolesAsync();
            var userRoles = user.UserRoles.Select(ur => ur.Role.Name.ToUpper()).ToHashSet();
            var selectedRoles = roleEditDto.RoleNames.Select(role => role.ToUpper()).ToHashSet();

            var rolesToAdd = roles.Where(role =>
            {
                var upperName = role.Name.ToUpper();
                return selectedRoles.Contains(upperName) && !userRoles.Contains(upperName);
            });

            if (!rolesToAdd.Any())
            {
                return Ok(mapper.Map<UserDto>(user));
            }

            user.UserRoles.AddRange(rolesToAdd.Select(role => new UserRole
            {
                Role = role
            }));

            var success = await userRepository.SaveAllAsync();

            if (!success)
            {
                return BadRequest("Failed to add roles.");
            }

            var userToReturn = mapper.Map<UserDto>(user);

            return Ok(userToReturn);
        }

        [Authorize(Policy = AuthorizationPolicyName.RequireAdminRole)]
        [HttpDelete("{id}/roles")]
        public async Task<ActionResult<UserDto>> RemoveRolesAsync(int id, [FromBody] EditRoleDto roleEditDto)
        {
            if (roleEditDto.RoleNames == null || roleEditDto.RoleNames.Length == 0)
            {
                return BadRequest("At least one role must be specified.");
            }

            var user = await userRepository.GetByIdAsync(id);
            var roles = await userRepository.GetRolesAsync();
            var userRoles = user.UserRoles.Select(ur => ur.Role.Name.ToUpper()).ToHashSet();
            var selectedRoles = roleEditDto.RoleNames.Select(role => role.ToUpper()).ToHashSet();

            var roleIdsToRemove = roles.Where(role =>
            {
                var upperName = role.Name.ToUpper();
                return selectedRoles.Contains(upperName) && userRoles.Contains(upperName);
            }).Select(role => role.Id).ToHashSet();

            if (roleIdsToRemove.Count == 0)
            {
                return Ok(mapper.Map<UserDto>(user));
            }

            user.UserRoles.RemoveAll(ur => roleIdsToRemove.Contains(ur.RoleId));
            var success = await userRepository.SaveAllAsync();

            if (!success)
            {
                return BadRequest("Failed to remove roles.");
            }

            var userToReturn = mapper.Map<UserDto>(user);

            return Ok(userToReturn);
        }
    }
}