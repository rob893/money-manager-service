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
using MoneyManagerService.Models.Domain;
using MoneyManagerService.Constants;

namespace MoneyManagerService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ServiceControllerBase
    {
        private readonly UserRepository userRepository;
        private readonly IMapper mapper;


        public UsersController(UserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CursorPaginatedResponse<UserForReturnDto, int>>> GetUsersAsync([FromQuery] CursorPaginationParameters searchParams)
        {
            var users = await userRepository.SearchAsync(searchParams);
            var paginatedResponse = CursorPaginatedResponse<UserForReturnDto, int>.CreateFrom(users, mapper.Map<IEnumerable<UserForReturnDto>>);

            return Ok(paginatedResponse);
        }

        [HttpGet("{id}", Name = "GetUserAsync")]
        public async Task<ActionResult<UserForReturnDto>> GetUserAsync(int id)
        {
            var user = await userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound($"User with id {id} does not exist.");
            }

            var userToReturn = mapper.Map<UserForReturnDto>(user);

            return Ok(userToReturn);
        }

        // [Authorize(Policy = "RequireAdminRole")]
        // [HttpGet("roles")]
        // public async Task<ActionResult<CursorPaginatedResponse<RoleForReturnDto, int>>> GetRolesAsync([FromQuery] CursorPaginationParameters searchParams)
        // {
        //     var roles = await userRepository.GetRolesAsync(searchParams);
        //     var paginatedResponse = CursorPaginatedResponse<RoleForReturnDto, int>.CreateFrom(roles, mapper.Map<IEnumerable<RoleForReturnDto>>);

        //     return Ok(paginatedResponse);
        // }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("{id}/roles")]
        public async Task<ActionResult<UserForReturnDto>> AddRolesAsync(int id, [FromBody] RoleEditDto roleEditDto)
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
                return Ok(mapper.Map<UserForReturnDto>(user));
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

            var userToReturn = mapper.Map<UserForReturnDto>(user);

            return Ok(userToReturn);
        }

        [Authorize(Policy = AuthorizationPolicyName.RequireAdminRole)]
        [HttpDelete("{id}/roles")]
        public async Task<ActionResult<UserForReturnDto>> RemoveRolesAsync(int id, [FromBody] RoleEditDto roleEditDto)
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
                return Ok(mapper.Map<UserForReturnDto>(user));
            }

            user.UserRoles.RemoveAll(ur => roleIdsToRemove.Contains(ur.RoleId));
            var success = await userRepository.SaveAllAsync();

            if (!success)
            {
                return BadRequest("Failed to remove roles.");
            }

            var userToReturn = mapper.Map<UserForReturnDto>(user);

            return Ok(userToReturn);
        }
    }
}