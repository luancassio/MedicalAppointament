using MedicalAppointment.Application.Auth.Cryptography;
using MedicalAppointment.Application.Auth.Token;
using MedicalAppointment.Application.Email;
using MedicalAppointment.Application.Service;
using MedicalAppointment.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointment.Application; 
public static class Bootstrapper {
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration) {
       // configuration passwordKey for encrypt passwaord
        services.AddScoped(opt => new EncryptPassword(configuration.GetSection("PasswordKey").Value));
        // configuration Email Send
        services.Configure<EmailConfig>(options => {
            options.Host_Address = configuration.GetSection("Email:Host_Address").Value;
            options.Host_Port = int.Parse(configuration.GetSection("Email:Host_Port").Value);
            options.Host_Username = configuration.GetSection("Email:Host_Username").Value;
            options.Host_Password = configuration.GetSection("Email:Host_Password").Value;
            options.Sender_EMail = configuration.GetSection("Email:Sender_EMail").Value;
            options.Sender_Name = configuration.GetSection("Email:Sender_Name").Value;
        });


        services.AddScoped<AutheticationService>();
        services.AddScoped<Token>();
        services.AddScoped<RoleService>();
        services.AddScoped<UserManager<User>>();
        services.AddScoped<EmailService>();
        services.AddScoped<SmsService>();

        //services.AddScoped<UserManager<User>, UserManager<User>>();
        //services.AddScoped<SignInManager<User>, SignInManager<User>>();
    }
}
