using MedicalAppointment.Application.Dto;
using MedicalAppointment.Application.Service;
using MedicalAppointment.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace MedicalAppointment.API.Controllers;
[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "admin")]

public class RoleController : ControllerBase {
    private readonly RoleService _roleService;
    public RoleController(RoleService roleService) {
        _roleService = roleService;
    }
    #region Create Role
    /// <summary>
    /// Cria uma nova função.
    /// </summary>
    /// <param name="role"></param>
    /// <returns>Um novo item criado</returns>
    /// <response code="201">Criado com sucesso.</response>
    /// <response code="400">caso não consiga criar ou se o campo for vazio ou null.</response>  
    #endregion     
    [HttpPost("create/roles", Name = "Create Role")]
    public async Task<IActionResult> CreateRole(string role) {
        if (role.IsNullOrEmpty()) {
            return this.StatusCode(StatusCodes.Status400BadRequest,
             new { sucess = false, message = "Função não pode ser vazio ou null!" });
        }
        object roleCreate = await _roleService.CreateRole(role.ToLower().Trim());
        return this.StatusCode(StatusCodes.Status201Created, roleCreate);
    }
    #region Get All Roles
    /// <summary>
    /// Busca todas Funções.
    /// </summary>

    /// <returns>Um novo item criado</returns>
    /// <response code="200">Caso consiga recuperar a lista de Funções</response>
    /// <response code="400">caso não consiga criar ou se o campo for vazio ou null.</response>  
    #endregion
    [HttpGet("roles", Name = "Get All Roles")]
    public async Task<IActionResult> GetAllRoles() {
        List<string> roles = await _roleService.GetAllRoles();
        return this.StatusCode(StatusCodes.Status200OK, roles);
    }
    #region Add Role in User
    /// <summary>
    /// Atribuir uma função para um usuário.
    /// </summary>
    /// <param name="role"></param>
    /// <param name="email"></param>
    /// <returns>Um novo item criado</returns>
    /// <response code="201">Retorna o novo item criado</response>
    /// <response code="400">caso não consiga criar ou se os campos forem vazio ou null.</response>  
    #endregion
    [HttpPost("add-role/user", Name = "Add Role in User")]
    public async Task<IActionResult> AddRoleInUser(string email, string role) {
        if (role.IsNullOrEmpty() || email.IsNullOrEmpty()) {
            return this.StatusCode(StatusCodes.Status400BadRequest,
             new { sucess = false, message = "Função é Email não podem ser vazio ou null!" });
        }
        UserDto roleCreate = await _roleService.AddRoleInUser(email.ToLower().Trim(), role.ToLower().Trim());
        return this.StatusCode(StatusCodes.Status201Created, roleCreate);
    }
    #region Get Roles User
    /// <summary>
    /// Buscar Funções atribuidas ao usuário.
    /// </summary>
    /// <param name="email" cref="string"></param>
    /// <returns>Um novo item criado</returns>
    /// <response code="201">Retorna o novo item criado</response>
    /// <response code="400">caso não consiga criar ou se o campo for vazio ou null.</response>  
    #endregion
    [HttpGet("get-roles/user", Name = "Get Roles User")]
    public async Task<IActionResult> GetUserRoles(string email) {
        if (email.IsNullOrEmpty()) {
            return this.StatusCode(StatusCodes.Status400BadRequest,
             new { sucess = false, message = "Email não pode ser vazio ou null!" });
            }
        IList<string> rolesUser = await _roleService.GetUserRoles(email.ToLower().Trim());
        return this.StatusCode(StatusCodes.Status200OK, rolesUser);
    }
    #region Remove Role in User
    /// <summary>
    /// Removendo uma função para um usuário.
    /// </summary>
    /// <param name="role"></param>
    /// <param name="email"></param>
    /// <returns>Um novo item criado</returns>
    /// <response code="202">Caso consiga deleta a função com sucesso.</response>
    /// <response code="400">caso não consiga criar ou se os campos forem vazio ou null.</response>  
    #endregion
    [HttpDelete("delete-role/user", Name = "Remove Role in User")]
    public async Task<IActionResult> RemoveUserRole(string email, string role) {
        if (role.IsNullOrEmpty() || email.IsNullOrEmpty()) {
            return this.StatusCode(StatusCodes.Status400BadRequest,
             new { sucess = false, message = "Função é Email não podem ser vazio ou null!" });
        }
        object removeRole = await _roleService.RemoveUserRole(email.ToLower().Trim(), role.ToLower().Trim());
        return this.StatusCode(StatusCodes.Status202Accepted, removeRole);
    }
    #region Remove Role
    /// <summary>
    /// Removendo uma função para um usuário.
    /// </summary>
    /// <param name="role"></param>
    /// <returns>Um novo item criado</returns>
    /// <response code="202">Caso consiga deleta a função com sucesso.</response>
    /// <response code="400">caso não consiga criar ou se o campo for vazio ou null.</response>  
    #endregion
    [HttpDelete("delete/role", Name = "Remove Role")]
    public async Task<IActionResult> RemoveRole(string role) {
        if (role.IsNullOrEmpty()) {
            return this.StatusCode(StatusCodes.Status400BadRequest,
             new { sucess = false, message = "Função não podem ser vazio ou null!" });
        }
        object removeRole = await _roleService.RemoveRole(role.ToLower().Trim());
        return this.StatusCode(StatusCodes.Status202Accepted, removeRole);
    }
}
