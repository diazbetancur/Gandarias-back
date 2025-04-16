using AutoMapper;
using CC.Domain.Dtos;
using CC.Domain.Entities;
using CC.Domain.Interfaces.Repositories;
using CC.Domain.Interfaces.Services;

namespace CC.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<RoleDto> AddRoleAsync(RoleDto roleDto)
        {
            bool roleExit = await _roleRepository.RoleExistsAsync(roleDto.Name);
            if (!roleExit)
            {
                Role role = new()
                {
                    Id = Guid.NewGuid(),
                    Name = roleDto.Name,
                    NormalizedName = roleDto.Name.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                };
                await _roleRepository.AddRoleAsync(role);
                return _mapper.Map<RoleDto>(role);
            }
            return null;
        }

        public Task<bool> DeleteRoleAsync(string roleId)
        {
            return _roleRepository.DeleteRoleAsync(roleId);
        }

        public async Task<bool> EditRoleAsync(RoleDto roleDto)
        {
            Role? roleToEdit = await _roleRepository.GetRoleByIdAsync(roleDto.Id.ToString());
            if (roleToEdit != null)
            {
                roleToEdit.Name = roleDto.Name;
                roleToEdit.NormalizedName = roleDto.Name.ToUpper();
                roleToEdit.ConcurrencyStamp = Guid.NewGuid().ToString();
                await _roleRepository.EditRoleAsync(roleToEdit);
                return true;
            }
            return false;
        }

        public async Task<List<RoleDto>> GetRolesAsync()
        {
            List<Role> roles = await _roleRepository.GetRolesAsync();
            return _mapper.Map<List<RoleDto>>(roles);
        }

        public async Task<List<RoleDto>> GetRolesByIdAsync(string roleName)
        {
            List<Role> roles = await _roleRepository.GetRolesByIdAsync(roleName);
            return _mapper.Map<List<RoleDto>>(roles);
        }
    }
}