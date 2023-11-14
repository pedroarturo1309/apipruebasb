using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace apipruebasb_repository.Usuario
{
    public static class CurrentUser
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static int UsuarioId
        {
            get
            {
                return int.Parse(GetClaimValue(_httpContextAccessor.HttpContext, "codigoUsuario") ?? "0");
            }
        }

        public static string CorreoElectronico
        {
            get
            {
                return GetClaimValue(_httpContextAccessor.HttpContext, "email") ?? "";
            }
        }

        public static string GetJwtToken(HttpContext httpContext)
        {
            var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            return token;
        }

        public static JwtPayload DecodeToken(HttpContext httpContext)
        {
            var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Payload;
        }

        public static string? GetClaimValue(HttpContext httpContext, string claimType)
        {
            var jwtToken = DecodeToken(httpContext);


            return jwtToken[claimType] != null ? jwtToken[claimType]?.ToString() : null;
        }

    }
}