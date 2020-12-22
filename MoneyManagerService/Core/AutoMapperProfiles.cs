using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using MoneyManagerService.Entities;
using MoneyManagerService.Models.DTOs;
using MoneyManagerService.Models.DTOs.Budget;
using MoneyManagerService.Models.DTOs.Expense;
using MoneyManagerService.Models.DTOs.Income;
using MoneyManagerService.Models.DTOs.TaxLiability;
using MoneyManagerService.Models.QueryParameters;

namespace MoneyManagerService.Core
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateCursorPaginationMaps();
            CreateUserMaps();
            CreateBudgetMaps();
            CreateExpenseMaps();
            CreateIncomeMaps();
            CreateTaxLiabilityMaps();
        }

        private void CreateCursorPaginationMaps()
        {
            CreateMap<CursorPaginationParameters, BudgetQueryParameters>();
        }

        private void CreateUserMaps()
        {
            CreateMap<User, UserDto>().ConstructUsing(x => new UserDto())
                .ForMember(dto => dto.Roles, opt =>
                    opt.MapFrom(u => u.UserRoles.Select(ur => ur.Role.Name)));
            CreateMap<RegisterUserDto, User>();
            CreateMap<Role, RoleDto>();
            CreateMap<JsonPatchDocument<UpdateUserDto>, JsonPatchDocument<User>>();
            CreateMap<Operation<UpdateUserDto>, Operation<User>>();
        }

        private void CreateBudgetMaps()
        {
            CreateMap<Budget, BudgetDto>();
            CreateMap<CreateBudgetDto, Budget>();
            CreateMap<JsonPatchDocument<UpdateBudgetDto>, JsonPatchDocument<Budget>>();
            CreateMap<Operation<UpdateBudgetDto>, Operation<Budget>>();
        }

        private void CreateExpenseMaps()
        {
            CreateMap<Expense, ExpenseDto>();
            CreateMap<CreateExpenseForBudgetDto, Expense>();
            CreateMap<CreateExpenseDto, Expense>();
            CreateMap<JsonPatchDocument<UpdateExpenseDto>, JsonPatchDocument<Expense>>();
            CreateMap<Operation<UpdateExpenseDto>, Operation<Expense>>();
        }

        private void CreateIncomeMaps()
        {
            CreateMap<Income, IncomeDto>();
            CreateMap<CreateIncomeDto, Income>();
            CreateMap<CreateIncomeForBudgetDto, Income>();
            CreateMap<JsonPatchDocument<UpdateIncomeDto>, JsonPatchDocument<Income>>();
            CreateMap<Operation<UpdateIncomeDto>, Operation<Income>>();
        }

        private void CreateTaxLiabilityMaps()
        {
            CreateMap<TaxLiability, TaxLiabilityDto>();
        }
    }
}