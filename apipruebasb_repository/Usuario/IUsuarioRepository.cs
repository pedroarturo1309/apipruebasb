using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apipruebasb_repository.Usuario.DTO;

namespace apipruebasb_repository.Usuario
{
    public interface IUsuarioRepository
    {
        public void VerificarInicioSesionOauth(UsuarioOauthDTO model);
        public GenericResponse<string> RegistrarUsuarioLocal(UsuarioOauthDTO model);
        public GenericResponse<string> IniciarSesionLocal(string email, string contrasena);
    }
}