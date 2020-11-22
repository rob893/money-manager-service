using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using MoneyManagerService.Entities;
using MoneyManagerService.Models.DTOs;
using MoneyManagerService.Models.DTOs.Budget;
using MoneyManagerService.Models.DTOs.Expense;

namespace MoneyManagerService.Core
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateUserMaps();
            CreateBudgetMaps();
            CreateExpenseMaps();
        }

        private void CreateUserMaps()
        {
            CreateMap<User, UserForReturnDto>().ConstructUsing(x => new UserForReturnDto())
                .ForMember(dto => dto.Roles, opt =>
                    opt.MapFrom(u => u.UserRoles.Select(ur => ur.Role.Name)));
            CreateMap<UserForRegisterDto, User>();
            CreateMap<Role, RoleForReturnDto>();
            CreateMap<JsonPatchDocument<UserForUpdateDto>, JsonPatchDocument<User>>();
            CreateMap<Operation<UserForUpdateDto>, Operation<User>>();
        }

        private void CreateBudgetMaps()
        {
            CreateMap<Budget, BudgetForReturnDto>();
            CreateMap<BudgetForCreateDto, Budget>();
            CreateMap<JsonPatchDocument<BudgetForUpdateDto>, JsonPatchDocument<Budget>>();
            CreateMap<Operation<BudgetForUpdateDto>, Operation<Budget>>();
        }

        private void CreateExpenseMaps()
        {
            CreateMap<Expense, ExpenseForReturnDto>();
            CreateMap<ExpenseForCreateDto, Expense>();
            CreateMap<JsonPatchDocument<ExpenseForUpdateDto>, JsonPatchDocument<Expense>>();
            CreateMap<Operation<ExpenseForUpdateDto>, Operation<Expense>>();
        }
    }
}