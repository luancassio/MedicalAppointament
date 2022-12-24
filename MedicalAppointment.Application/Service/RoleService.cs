using AutoMapper;
using MedicalAppointment.Application.Dto;
using MedicalAppointment.Domain.Entity;
using MedicalAppointment.Persistence.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointment.Application.Service; 
public class RoleService {
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public RoleService(
        UserManager<User> userManager, 
        RoleManager<IdentityRole> roleManager,
        DataContext dataContext,
        IMapper mapper
    ) {
        _roleManager = roleManager;
        _userManager = userManager;
        _dataContext = dataContext;
        _mapper = mapper;
    }
    public async Task<List<string>> GetAllRoles() {
        List<IdentityRole> roles = await _roleManager.Roles.ToListAsync();
        List<string> listNameRoles = new List<string>();
        foreach (var role in roles) { listNameRoles.Add(role.Name.ToString()); };
        return listNameRoles;
    }
    public async Task<Object> CreateRole(string role) {
        //check if the role
        bool roleExist = await _roleManager.RoleExistsAsync(role);
        if (!roleExist) {
            //role no exist create role
            IdentityResult roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
            if (roleResult.Succeeded) {
                return new { sucess = true, message = $"Função {role} foi criada com sucesso!" };
            }
        }
        throw new Exception("Função já  existe na base!");
    }
    public async Task<UserDto> AddRoleInUser(string email, string role) {
        //check
        User user = await FindByUser(email);
        bool roleExist = await RoleExistsAsync(role);
        
        try {
            if (roleExist) {
                bool listRolesUser = await _userManager.IsInRoleAsync(user, role);
                if (!listRolesUser) {
                    IdentityResult addToRole = await _userManager.AddToRoleAsync(user, role);

                    if (addToRole.Succeeded) {
                        UserDto userDto = _mapper.Map<UserDto>(user);
                        userDto.Password = "";
                        return userDto;
                    }
                }
            }
        } catch (Exception ex) {
            throw new Exception($"Essa função já existe nesse usuário!{ex.Message}");
        }
        throw new Exception("Usuário não foi capaz adicionar a função!");
    }
    public async Task<IList<string>> GetUserRoles(string email) {
        //check                                                                                                                  User user = await FindByUser(email);
        User user = await FindByUser(email);

        if (user == null) {
            throw new Exception("Usuário não existe na base!");
        }
        IList<string> roles = await _userManager.GetRolesAsync(user);
        return roles;
    }
    public async Task<Object> RemoveUserRole(string email, string role) {
        //check
        User user = await FindByUser(email);
        await RoleExistsAsync(role);

        try {
            IdentityResult removeRole = await _userManager.RemoveFromRoleAsync(user, role);
            if (removeRole.Succeeded) {
                return new { sucess = true, message = "Função removida com sucesso!" };
            }
        } catch (Exception ex) { 
        throw new Exception($"Erro ao tentar remover função do usuário {user.UserName}!, {ex.Message}");
        }
        throw new Exception($"Erro ao tentar remover função do usuário {user.UserName}!");
    }
    public async Task<Object> RemoveRole(string role) {
        //check
        IdentityRole roleCurrent = new IdentityRole(role);
        try {
            List<IdentityRole> roles = await _roleManager.Roles.ToListAsync();
            IdentityRole selectRole = roles.Find(role => role.Name == roleCurrent.Name);
            if (selectRole != null) {
                IdentityResult result = await _roleManager.DeleteAsync(selectRole);
                if (result.Succeeded) {
                    return new { sucess = true, message = $"Função {role}, foi criada com sucesso!" };
                }
            }
            throw new Exception($"Erro ao tentar remover função {role}!");
        } catch (Exception ex) {
            throw new Exception($"Erro ao tentar remover função {role}!, {ex.Message}");
        }
        throw new Exception($"Erro ao tentar remover função {role}!");
    }
    // method private 
    public async Task<User> FindByUser(string email) {
        try {
            User user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.FindByIdAsync(user.Id) != null) {
                return user;
            }
        } catch (Exception ex) {
            throw new Exception("Usuário já existe na base!", ex);
        }
        return null;
    }
    public async Task<bool> RoleExistsAsync(string role) {
        try {
            bool roleExists = await _roleManager.RoleExistsAsync(role);
            if (roleExists) {
                return true;
            }
        } catch (Exception ex) { 
            throw new Exception("Função não existe na base!", ex);
        }
        return false;
    }

}