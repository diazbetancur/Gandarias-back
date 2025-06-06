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
            CreateMap<UserActivityLog, UserActivityLogDto>().ReverseMap();
            CreateMap<ShiftTypeDto, ShiftType>().ReverseMap();
            CreateMap<License, LicenseDto>().ReverseMap();
            CreateMap<UserWorkstation, UserWorkstationDto>().ReverseMap()
            .ForMember(dest => dest.Workstation, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());
            CreateMap<HybridWorkstation, HybridWorkstationDto>().ReverseMap();
            CreateMap<UserRestrictionShift, UserRestrictionShiftDto>().ReverseMap();
            CreateMap<EmployeeScheduleRestriction, EmployeeScheduleRestrictionDto>().ReverseMap();
            CreateMap<AbsenteeismType, AbsenteeismTypeDto>().ReverseMap();
            CreateMap<UserAbsenteeism, UserAbsenteeismDto>().ReverseMap()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.AbsenteeismType, opt => opt.Ignore());
            CreateMap<EmployeeScheduleException, EmployeeScheduleExceptionDto>().ReverseMap();
            CreateMap<LawRestriction, LawRestrictionDto>().ReverseMap();
            CreateMap<WorkstationDemand, WorkstationDemandDto>().ReverseMap();
            CreateMap<WorkstationDemandTemplate, WorkstationDemandTemplateDto>().ReverseMap();
        }
    }
}