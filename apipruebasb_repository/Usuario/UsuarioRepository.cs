using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apipruebasb_repository.Usuario.DTO;

namespace apipruebasb_repository.Usuario
{
    public class UsuarioRepository : IUsuarioRepository
    {

        private readonly PruebasbDBContext _context;

        public UsuarioRepository(PruebasbDBContext context)
        {
            _context = context;
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