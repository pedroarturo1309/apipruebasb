using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apipruebasb_repository.Usuario.DTO;
using BCrypt.Net;
namespace apipruebasb_repository.Usuario
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly PruebasbDBContext _context;

        public UsuarioRepository(PruebasbDBContext context)
        {
            _context = context;
        }

        public GenericResponse<string> IniciarSesionLocal(string email, string contrasena)
        {

            GenericResponse<string> respuesta = new GenericResponse<string>();
            var usuario = _context.Usuarios.FirstOrDefault(x => x.CorreoElectronico == email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(contrasena, usuario.Contrasena))
                respuesta.AddNotification("Usuario y/o contraseña incorrectos.");

            return respuesta;
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

        public void VerificarInicioSesionOauth(UsuarioOauthDTO model)
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
        }
    }
}