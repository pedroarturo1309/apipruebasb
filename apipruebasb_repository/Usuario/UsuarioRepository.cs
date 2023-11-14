using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using apipruebasb_repository.Usuario.DTO;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
namespace apipruebasb_repository.Usuario
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly PruebasbDBContext _context;
        private readonly IConfiguration _configuration;

        public UsuarioRepository(PruebasbDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public GenericResponse<string> IniciarSesionLocal(string email, string contrasena)
        {

            GenericResponse<string> respuesta = new GenericResponse<string>();
            var usuario = _context.Usuarios.FirstOrDefault(x => x.CorreoElectronico == email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(contrasena, usuario.Contrasena))
                respuesta.AddNotification("Usuario y/o contraseña incorrectos.");

            string token = GenerarTokenJWT(new UsuarioOauthDTO() { Apellidos = usuario.Apellidos, Nombres = usuario.Nombres, CorreoElectronico = usuario.CorreoElectronico }, usuario.Id);

            respuesta.Data = token;

            return respuesta;
        }

        public string GenerarTokenJWT(UsuarioOauthDTO user, int codigoUsuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Key"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("nombre", user.Nombres),
                    new Claim("apellido", user.Apellidos),
                    new Claim(ClaimTypes.Email, user.CorreoElectronico),
                    new Claim("codigoUsuario", codigoUsuario.ToString())
                     }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public GenericResponse<string> RegistrarUsuarioLocal(UsuarioOauthDTO model)
        {
            GenericResponse<string> respuesta = new GenericResponse<string>();

            try
            {
                if (_context.Usuarios.Any(x => x.CorreoElectronico == model.CorreoElectronico))
                    respuesta.AddNotification("Ya existe una cuenta registrada con este correo electronico.");


                // hash password
                string contrasenaEncriptada = BCrypt.Net.BCrypt.HashPassword(model.Contrasena);

                // save user
                _context.Usuarios.Add(new apipruebasb_entities.Usuarios.Usuarios()
                {
                    Apellidos = model.Apellidos,
                    CorreoElectronico = model.CorreoElectronico,
                    CuentaRegistradaDesde = "LOCAL",
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    Nombres = model.Nombres,
                    Contrasena = contrasenaEncriptada
                });
                _context.SaveChanges();
            }
            catch
            {
                respuesta.AddNotification("Ha ocurrido un error al crear el usuario, favor intentelo mas tarde");
            }


            return respuesta;
        }

        public string VerificarInicioSesionOauth(UsuarioOauthDTO model)
        {
            if (!_context.Usuarios.Any(x => x.CorreoElectronico == model.CorreoElectronico))
            {
                _context.Usuarios.Add(new apipruebasb_entities.Usuarios.Usuarios()
                {
                    Apellidos = model.Apellidos,
                    CorreoElectronico = model.CorreoElectronico,
                    CuentaRegistradaDesde = model.AutenticadoDesde,
                    FechaCreacion = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    Nombres = model.Nombres
                });

                _context.SaveChanges();
            }

            string token = GenerarTokenJWT(model, _context.Usuarios.First(x => x.CorreoElectronico == model.CorreoElectronico).Id);

            return token;
        }

        public UsuarioOauthDTO? ValidateJwtToken(string? token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Key"]!);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                // var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "codigoUsuario").Value);

                var user = new UsuarioOauthDTO() {
                    Apellidos = jwtToken.Claims.First(x => x.Type == "apellido").Value,
                    Nombres = jwtToken.Claims.First(x => x.Type == "nombre").Value,
                    CorreoElectronico = jwtToken.Claims.First(x => x.Type == "email").Value,
                };

                // return user id from JWT token if validation successful
                return user;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                // return null if validation fails
                return null;
            }
        }
    }
}