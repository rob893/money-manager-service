using System.Linq;
using AutoMapper;
using MoneyManagerService.Models.Domain;
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
        }

        private void CreateBudgetMaps()
        {
            CreateMap<BudgetForCreateDto, Budget>();
        }

        private void CreateExpenseMaps()
        {
            CreateMap<ExpenseForCreateDto, Expense>();
        }
    }
}