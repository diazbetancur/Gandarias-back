using AutoMapper;
using CC.Domain.Dtos;
using CC.Domain.Entities;

namespace CC.Domain
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<LoginUserDto, User>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<WorkArea, WorkAreaDto>().ReverseMap();
            CreateMap<Workstation, WorkstationDto>().ReverseMap();
            CreateMap<HireType, HireTypeDto>().ReverseMap();
        }
    }
}