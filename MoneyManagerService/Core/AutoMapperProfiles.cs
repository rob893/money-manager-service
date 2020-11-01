using System.Linq;
using AutoMapper;
using MoneyManagerService.Models.Domain;
using MoneyManagerService.Models.DTOs;

namespace MoneyManagerService.Core
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateUserMaps();
        }

        private void CreateUserMaps()
        {
            CreateMap<User, UserForReturnDto>().ConstructUsing(x => new UserForReturnDto())
                .ForMember(dto => dto.Roles, opt =>
                    opt.MapFrom(u => u.UserRoles.Select(ur => ur.Role.Name)));
            CreateMap<UserForRegisterDto, User>();

            CreateMap<Role, RoleForReturnDto>();
        }
    }
}