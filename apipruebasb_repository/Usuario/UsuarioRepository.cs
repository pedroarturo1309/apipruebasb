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

        public void RegistrarUsuario()
        {
            throw new NotImplementedException();
        }

        private bool validateUser(UsuarioOauthDTO model)
        {
            if(_context.Usuarios.Any(x => x.CorreoElectronico == model.CorreoElectronico))

            return false;
        }
    }
}