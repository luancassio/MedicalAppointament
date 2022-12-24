using AutoMapper;
using MedicalAppointment.Application.Auth.Cryptography;
using MedicalAppointment.Application.Auth.Token;
using MedicalAppointment.Application.Dto;
using MedicalAppointment.Application.Email;
using MedicalAppointment.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using static System.Net.WebRequestMethods;

namespace MedicalAppointment.Application.Service;
public class AutheticationService {
    private readonly UserManager<User> _userManager;
    private readonly EncryptPassword _encryptPassword;
    private readonly SignInManager<User> _signInManager;
    private readonly IMapper _mapper;
    private readonly Token _tokenController;
    private readonly EmailService _emailSend;


    public AutheticationService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        EncryptPassword encryptPassword,
        IMapper mapper,
        Token tokenController,
        EmailService emailSend
    ) {
        _userManager = userManager;
        _encryptPassword = encryptPassword;
        _mapper = mapper;
        _tokenController = tokenController;
        _signInManager = signInManager;
        _emailSend = emailSend;
    }
    public async Task<List<UserDto>> GetAllUsers() {
        try {
            List<User> users = await _userManager.Users.ToListAsync();
            users.ForEach(u => u.UserName = u.UserName.Replace(u.Id, ""));

            List<UserDto> userDto = _mapper.Map<List<UserDto>>(users);

            userDto.ForEach(u => u.Password = "");
            return userDto;

        } catch (Exception ex) {
            throw new Exception("Erro ao tentar buscar todos os usuários!", ex);
        }
    }
    public async Task<Object> CreateUser(UserDto user) {
        //check
        try {
            User userFind = await _userManager.FindByEmailAsync(user.Email);
            if (userFind == null) {
                //turning userdto into user
                User userCurrent = _mapper.Map<User>(user);

                userCurrent.Id = Guid.NewGuid().ToString();

                string newPassword = _encryptPassword.Encrypt(user.Password);
                userCurrent.Password = newPassword;
                userCurrent.UserName = $"{user.UserName}{userCurrent.Id}";
                userCurrent.Create_At = DateTime.Now;
                userCurrent.Update_At = DateTime.Now;
                userCurrent.isActive = true;


                IdentityResult userCreate = await _userManager.CreateAsync(userCurrent, userCurrent.Password);

                if (userCreate.Succeeded) {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(userCurrent);
                    userCurrent.UserName = userCurrent.UserName.Replace(userCurrent.Id, "");
                    UserDto userDto = _mapper.Map<UserDto>(userCurrent);
                    userDto.Password = "";

                    //content email
                    StringBuilder message = new StringBuilder();
                    message.Append($"Olá, {user.Name} Seja bem vindo!");
                    message.Append($"<br>");
                    message.Append($"<br>");
                    message.Append($"Recebemos uma solicitação para criar sua conta de acesso em nosso site.");
                    message.Append($"<br>");
                    message.Append($"Ela ocorreu em {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}.");
                    message.Append($"<br>");
                    message.Append($"Se você reconhece essa ação, clique no link abaixo para prosseguir:");
                    message.Append($"<br>");
                    message.Append($"{"http://URL_DO_FRONT/?"}token={token}&email={user.Email}");
                    message.Append($"<br>");
                    message.Append($"<br>");
                    message.Append($"Atenciosamente,");
                    message.Append($"<br>");
                    message.Append($"Medical Appointament.");

                    //string resetURL = $"Olá, {user.Name},\r\n\r\n\r\n\r\n" +
                    //    $"Recebemos uma solicitação para restaurar sua senha de acesso em nosso site.\r\n\r\n" +
                    //    $"Ela ocorreu em {DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss")}.\r\n\r\n" +
                    //    $"Se você reconhece essa ação, clique no link abaixo para prosseguir:\n" +
                    //    $"{"http://URL_DO_FRONT/?"}token={token}&email={user.Email}";

                    //TODO: here to in the code for send email
                    EmailSend emailSend = new EmailSend();
                    emailSend.ToAddress = user.Email;
                    emailSend.Subject = "Confirmar Email";
                    emailSend.Body = message.ToString();

                    await _emailSend.SendEmailAsync(emailSend);

                    return new
                    {
                        success = true,
                        message = $"Link para validar seu email foi enviado com sucesso! \n {"http://URL_DO_FRONT/?"}token={token}&email={user.Email}"
                    }; ;
                }
                return null;
            }

        } catch (Exception ex) {
            throw new Exception("Email já existe na base de dados!", ex);
        }
        throw new Exception("Email já existe na base de dados!");
    }
    public async Task<string> AuthUser(Login login) {
        //check
        try {
            User userFind = await _userManager.FindByEmailAsync(login.Email);
            if (userFind != null) {
                if (!await _userManager.IsLockedOutAsync(userFind)) {
                    //check password
                    string newPassword = _encryptPassword.Encrypt(login.Password);
                    bool checkPassword = await _userManager.CheckPasswordAsync(userFind, newPassword);

                    if (await _userManager.IsEmailConfirmedAsync(userFind))
                    {
                        if (checkPassword)
                        {
                            //generate token
                            string token = await _tokenController.GenerateToken(userFind);
                            await _userManager.ResetAccessFailedCountAsync(userFind);

                            return $"Bearer {token}";
                        }
                        else
                        {
                            await _userManager.AccessFailedAsync(userFind);

                            if (await _userManager.IsLockedOutAsync(userFind))
                            {
                                var token = await _userManager.GeneratePasswordResetTokenAsync(userFind);
                                //var resetURL = new { url = "UrlDoFront", token = token, email = user.Email };
                                StringBuilder message = new StringBuilder();
                                message.Append($"Olá, {userFind.Name}");
                                message.Append($"<br>");
                                message.Append($"<br>");
                                message.Append($"Percebemos que houve algumas tentativas de acessar o site com seu usuário.");
                                message.Append($"<br>");
                                message.Append($"Ela ocorreu em {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}.");
                                message.Append($"<br>");
                                message.Append($"Devido essa ação suspeita seu usuário foi bloqueado, clique no link abaixo para redefinir sua senha:");
                                message.Append($"<br>");
                                message.Append($"{"http://URL_DO_FRONT/?"}token={token}&email={userFind.Email}");
                                message.Append($"<br>");
                                message.Append($"<br>");
                                message.Append($"Atenciosamente,");
                                message.Append($"<br>");
                                message.Append($"Medical Appointament.");

                                //string resetURL = $"Olá, {user.Name},\r\n\r\n\r\n\r\n" +
                                //    $"Recebemos uma solicitação para restaurar sua senha de acesso em nosso site.\r\n\r\n" +
                                //    $"Ela ocorreu em {DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss")}.\r\n\r\n" +
                                //    $"Se você reconhece essa ação, clique no link abaixo para prosseguir:\n" +
                                //    $"{"http://URL_DO_FRONT/?"}token={token}&email={user.Email}";

                                //TODO: here to in the code for send email
                                EmailSend emailSend = new EmailSend();
                                emailSend.ToAddress = userFind.Email;
                                emailSend.Subject = "Usuário Bloqueado";
                                emailSend.Body = message.ToString();

                                await _emailSend.SendEmailAsync(emailSend);
                            }
                            return "Senha incorreta!";
                        }
                    }
                    else
                    {
                        return $"Acesse o link que foi enviado ao seu email para confirmar seu cadastro!";
                    }
                }
                else
                {
                    return "Seu usuário está bloqueado!";
                }
            }
            else
            {
                return "Usuário não encontrado!";

            }

        } catch (Exception ex) {
            throw new Exception("Erro ao tentar criar Token!", ex);
        }
    }
    public async Task<Object> DeleteUser(string email) {
        try {
            //check
            User userFind = await _userManager.FindByEmailAsync(email);
            if (userFind != null) {
                IdentityResult result = await _userManager.DeleteAsync(userFind);

                if (result.Succeeded) {
                    return new { sucess = true, message = $"Usuário {userFind.Name} foi deletado com sucesso!" };
                } else {
                    return new { sucess = false, message = $"Erro ao tentar deletar usuário!" };
                }
            }
        } catch (Exception ex) {
            throw new Exception("Erro ao tentar deletar usuário!", ex);
        }
        throw new Exception("Erro ao tentar deletar usuário!");
    }
    public async Task<Object> UpdateUser(UserDto user) {
        try {
            //check
            User userFind = await _userManager.FindByEmailAsync(user.Email);

            if (userFind != null) {
                userFind.Update_At = DateTime.Now;
                userFind.Password = _encryptPassword.Encrypt(user.Password);
                userFind.Name = user.Name;
                userFind.UserName = user.UserName;
                IdentityResult result = await _userManager.UpdateAsync(userFind);

                if (result.Succeeded) {
                    return new { sucess = true, message = $"Usuário {userFind.Name} foi atualizado com sucesso!" };
                } else {
                    return new { sucess = false, message = $"Erro ao tentar atualizar usuário!" };
                }
            }
        } catch (Exception ex) {
            throw new Exception("Erro ao tentar atualizar usuário!", ex);
        }
        throw new Exception("Erro ao tentar atualizar usuário!");
    }

    //! Forgot Passeord and  Reset Password
    public async Task<Object> ForgotPassword(ForgotPassword forgotPassword) {

        try {
            var user = await _userManager.FindByEmailAsync(forgotPassword.Email);
            if (user != null) {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var resetURL = new { url = "UrlDoFront", token = token, email = user.Email };
                StringBuilder message = new StringBuilder();
                message.Append($"Olá, {user.Name}");
                message.Append($"<br>");
                message.Append($"<br>");
                message.Append($"Recebemos uma solicitação para restaurar sua senha de acesso em nosso site.");
                message.Append($"<br>");
                message.Append($"Ela ocorreu em {DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}.");
                message.Append($"<br>");
                message.Append($"Se você reconhece essa ação, clique no link abaixo para prosseguir:");
                message.Append($"<br>");
                message.Append($"{"http://URL_DO_FRONT/?"}token={token}&email={user.Email}");
                message.Append($"<br>");
                message.Append($"<br>");
                message.Append($"Atenciosamente,");
                message.Append($"<br>");
                message.Append($"Medical Appointament.");

                //string resetURL = $"Olá, {user.Name},\r\n\r\n\r\n\r\n" +
                //    $"Recebemos uma solicitação para restaurar sua senha de acesso em nosso site.\r\n\r\n" +
                //    $"Ela ocorreu em {DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss")}.\r\n\r\n" +
                //    $"Se você reconhece essa ação, clique no link abaixo para prosseguir:\n" +
                //    $"{"http://URL_DO_FRONT/?"}token={token}&email={user.Email}";

                //TODO: here to in the code for send email
                EmailSend emailSend = new EmailSend();
                emailSend.ToAddress = user.Email;
                emailSend.Subject = "Esqueceu sua senha";
                emailSend.Body = message.ToString();

                await _emailSend.SendEmailAsync(emailSend);
                //File.WriteAllText("LinkResetPassword.txt", resetURL.ToString());
                return new {
                    success = true,
                    message = $"Link para resetar sua senha foi enviado com sucesso! \n {"http://URL_DO_FRONT/?"}token={token}&email={user.Email}"
                };
            }
        } catch (Exception) {

            throw new Exception("Error ao tentar gerar link de reset de senha!");
        }
        return null;
    }
    public async Task<Object> ResetPassword(ResetPassword resetPassword) {

        try {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            user.LockoutEnd = null;
            if (user != null) {
                var token = await _userManager.ResetPasswordAsync(user, resetPassword.Token, _encryptPassword.Encrypt(resetPassword.Password));
                
                await _userManager.ResetAccessFailedCountAsync(user);
                return new {
                    success = true,
                    message = "Senha resetada com sucesso!"
                };
            }
        } catch (Exception) {

            throw new Exception("Error ao tentar gerar link de reset de senha!");
        }
        return null;
    }
    public async Task<Object> ConfirmerEmail(string token, string email)
    {

        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return new
                    {
                        success = true,
                        message = "Conta validada com sucesso!"
                    };
                }
                else
                {
                    return new
                    {
                        success = false,
                        message = "Error ao tentar validar conta, entre em contato com administrador!"
                    };
                }
            }
            else
            {
                return new
                {
                    success = false,
                    message = "Usuário não encontrado, entre em contato com administrador!"
                };
            }
        }
        catch (Exception)
        {

            throw new Exception("Error ao tentar validar conta!");
        }
    }
}