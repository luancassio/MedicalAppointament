using MedicalAppointment.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MedicalAppointment.Application.Auth.Token;
public class Token {
    private const string eml = nameof(eml);
    private double _expiryTimeInMinutes;
    private string _securityKey;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;


    public Token( 
        IConfiguration configuration, 
        UserManager<User> userManager, 
        RoleManager<IdentityRole> roleManager) {

        _expiryTimeInMinutes = int.Parse(configuration.GetSection("TimeToken").Value);
        _securityKey = configuration.GetSection("TokenKey").Value;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<string> GenerateToken(User user) {
        List<Claim> claims = await GetAllValidClaims(user);

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        SecurityTokenDescriptor tokenDesc = new SecurityTokenDescriptor { 
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_expiryTimeInMinutes),
            SigningCredentials = new SigningCredentials (SymmetricKey(), SecurityAlgorithms.HmacSha256Signature)
        };
        SecurityToken securityToken = tokenHandler.CreateToken(tokenDesc);
        string token =  tokenHandler.WriteToken(securityToken);

        return token;
    }
    private async Task<List<Claim>> GetAllValidClaims(User user) {

        var claims = new List<Claim> {
            new Claim("Id", user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("eml", user.Email)
        };
        IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        IList<string> userRoles = await _userManager.GetRolesAsync(user);
        foreach (string role in userRoles) {
            claims.Add(new Claim(ClaimTypes.Role, role));
            IdentityRole roleCurrent = await _roleManager.FindByNameAsync(role);
            if(roleCurrent != null) {
                IList<Claim> roleClaims = await _roleManager.GetClaimsAsync(roleCurrent);
                foreach (Claim roleclaim in roleClaims) {
                    claims.Add(roleclaim);
                }
            }
        }
        return claims;
    }
    public void TokenValidator(string token) {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters {
            RequireExpirationTime = true,
            IssuerSigningKey = SymmetricKey(),
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = false,
            ValidateAudience = false
        };
        tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
    }
    private SymmetricSecurityKey SymmetricKey() {
        var symmetricKey = Convert.FromBase64String(_securityKey);
        return new SymmetricSecurityKey (symmetricKey);
    }

}
