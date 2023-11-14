using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apipruebasb_repository.Usuario;
using Microsoft.AspNetCore.Http;

namespace apipruebasb_repository.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUsuarioRepository userService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var user = userService.ValidateJwtToken(token);
            if (user != null)
            {
                // attach user to context on successful jwt validation
                context.Items["User"] = user;
            }

            await _next(context);
        }
    }
}