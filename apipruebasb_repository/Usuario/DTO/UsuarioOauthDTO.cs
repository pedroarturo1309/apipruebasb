using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apipruebasb_repository.Usuario.DTO
{
    public class UsuarioOauthDTO
    {
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string AutenticadoDesde { get; set; }
        public string CorreoElectronico { get; set; }
        public string? Contrasena { get; set; }
    }
}