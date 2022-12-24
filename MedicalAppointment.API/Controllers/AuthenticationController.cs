using MedicalAppointment.Application.Dto;
using MedicalAppointment.Application.Service;
using MedicalAppointment.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace MedicalAppointment.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase {
    private readonly AutheticationService _userAuthenticationService;


    public AuthenticationController(AutheticationService userAuthenticationService) {
        _userAuthenticationService = userAuthenticationService;
    }
    #region Get All Users
    /// <summary>
    /// Busca todos os usuários.
    /// </summary>

    /// <returns>Um novo item criado</returns>
    /// <response code="200">Caso usuário esteja autenticado é autorizado ele vai conseguir recuperar a lista de usuários.</response>
    /// <response code="401">Se o usuário não tiver autenticado.</response>
    /// <response code="403">Se o usuário tiver autenticado mas não tiver autorização.</response>  
    /// 
    #endregion
    [HttpGet("users", Name = "Get All Users")]
    //[Authorize(Roles = "admin, user")]
    public async Task<IActionResult> GetAllUsers() {
        List<UserDto> users = await _userAuthenticationService.GetAllUsers();
        return this.StatusCode(StatusCodes.Status200OK, users);
    }
    #region Create User
    /// <summary>
    /// Cria um novo usuário.
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Um novo item criado</returns>
    /// <response code="201">Retorna o novo item criado</response>
    /// <response code="400">Se o item não for criado</response>  
    #endregion
    //[Authorize(Roles = "admin")]
    [HttpPost("create/user", Name = "Create User")]
    public async Task<IActionResult> CreateUser(UserDto user) {
        //if (login.Email.IsNullOrEmpty() || login.Password.IsNullOrEmpty()) {
        //    return this.StatusCode(StatusCodes.Status400BadRequest,
        //     new { sucess = false, message = "Email não podem ser vazio ou null!" });
        //}
        object userCreate = await _userAuthenticationService.CreateUser(user);
        return this.StatusCode(StatusCodes.Status201Created, userCreate);
    }
    #region Auth User
    /// <summary>
    /// Criar token do usuário.
    /// </summary>
    /// <remarks>
    /// Exemplo:
    ///
    ///     POST /auth/user
    ///     {
    ///        "email": "email@email.com",
    ///        "password": "P@ssword123"
    ///     }
    ///
    /// </remarks>
    /// <param name="login"></param>
    /// <returns>Um novo item criado</returns>
    /// <response code="201 - Created">Retorna o novo item criado</response>
    /// <response code="400">caso não consiga criar ou se o campos forem vazio ou null.</response>  
    /// 
    #endregion
    [HttpPost("auth/user", Name = "Auth User")]
    public async Task<IActionResult> AuthUser(Login login) {
        //if (login.Email.IsNullOrEmpty() || login.Password.IsNullOrEmpty()) {
        //    return this.StatusCode(StatusCodes.Status400BadRequest,
        //     new { sucess = false, message = "Email não podem ser vazio ou null!" });
        //}

        login.Email.Trim();
        login.Password.Trim();

        string token = await _userAuthenticationService.AuthUser(login);
        return this.StatusCode(StatusCodes.Status201Created, token);
    }
    #region Auth User
    /// <summary>
    /// Criar token do usuário.
    /// </summary>
    /// <param name="email"></param>
    /// <returns>Um novo item criado</returns>
    /// <response code="202">Caso consiga deleta o usuário com sucesso.</response>
    /// <response code="400">caso não consiga criar ou se o campo for vazio ou null.</response> 
    #endregion
    [Authorize(Roles = "admin")]
    [HttpDelete("delete/user", Name = "Delete User")]
    public async Task<IActionResult> DeleteUser(string email) {
        if (email.IsNullOrEmpty()) {
            return this.StatusCode(StatusCodes.Status400BadRequest,
             new { sucess = false, message = "Email não podem ser vazio ou null!" });
        }
        object deleteUser = await _userAuthenticationService.DeleteUser(email.Trim());
        return this.StatusCode(StatusCodes.Status202Accepted, deleteUser);
    }
    #region Update User
    /// <summary>
    /// Atualizar usuário.
    /// </summary>
    /// <param name="user"></param>
    /// <returns>Um novo item criado</returns>
    /// <response code="201">Caso consiga atualizar o usuário com sucesso.</response>
    /// <response code="400">caso não consiga criar ou se o campo for vazio ou null.</response> 
    #endregion
    [Authorize(Roles = "admin, user")]
    [HttpPut("update/user", Name = "Atualizar User")]
    public async Task<IActionResult> UpdateUser(UserDto user) {
        object deleteUser = await _userAuthenticationService.UpdateUser(user);
        return this.StatusCode(StatusCodes.Status201Created, deleteUser);
    }
    #region Forgot Password
    /// <summary>
    /// Quando o Usuário esquecer a senha manda um link para se email.
    /// </summary>
    /// <param name="forgotPassword"></param>
    /// <returns>Um novo item criado</returns>
    /// <response code="201">Caso consiga gerar o link para resetar sua senha.</response>
    /// <response code="400">caso não consiga gerar o link ou se o campo for vazio ou null.</response> 
    #endregion
    //[Authorize(Roles = "admin, user")]
    [HttpPost("forgot/password", Name = "Forgot Password User")]
    public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword) {
        var user = await _userAuthenticationService.ForgotPassword(forgotPassword);
        if(user != null) {
           return this.StatusCode(StatusCodes.Status201Created, user);
        }
        return this.StatusCode(StatusCodes.Status400BadRequest, 
            new { success = false, message = "Error ao tentar gerar link de reset de senha!" });
    }
    #region reset Password
    /// <summary>
    /// validar as novas credenciais do usuário.
    /// </summary>
    /// <param name="resetPassword"></param>
    /// <returns>Um novo item criado</returns>
    /// <response code="201">Caso consiga gerar o link para resetar sua senha.</response>
    /// <response code="400">caso não consiga gerar o link ou se o campo for vazio ou null.</response> 
    #endregion
    //[Authorize(Roles = "admin, user")]
    [HttpPost("Reset/password", Name = "Reset Password User")]
    public async Task<IActionResult> ResetPassword(ResetPassword resetPassword) {
        var user = await _userAuthenticationService.ResetPassword(resetPassword);
        if (user != null) {
            return this.StatusCode(StatusCodes.Status201Created, user);
        }
        return this.StatusCode(StatusCodes.Status400BadRequest,
            new { success = false, message = "Error ao tentar gerar link de reset de senha!" });
    }
    [HttpGet("confirmer/email", Name = "Confirme Email User")]
    public async Task<IActionResult> ConfirmerEmail(string token, string email)
    {
        var user = await _userAuthenticationService.ConfirmerEmail(token, email);
        if (user != null)
        {
            return this.StatusCode(StatusCodes.Status201Created, user);
        }
        return this.StatusCode(StatusCodes.Status400BadRequest,
            new { success = false, message = "Error ao tentar gerar link de reset de senha!" });
    }
}
