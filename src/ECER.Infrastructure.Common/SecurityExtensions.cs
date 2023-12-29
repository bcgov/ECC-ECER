using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace ECER.Infrastructure.Common;

public static class SecurityExtensions
{
    private static JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

    public static JwtSecurityToken? DecryptToken(this string rawToken, TokenValidationParameters tokenValidationParameters)
    {
        jwtSecurityTokenHandler.ValidateToken(rawToken, tokenValidationParameters, out var token);
        return token as JwtSecurityToken;
    }
}