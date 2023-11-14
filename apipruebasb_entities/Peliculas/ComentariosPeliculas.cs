using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using apipruebasb_entities.Usuarios;

namespace apipruebasb_entities.Peliculas
{
    [Table("ComentariosPeliculas")]
    public class ComentariosPeliculas
    {
        public int Id { get; set; }

        public string PeliculaId { get; set; }
        public string Comentario { get; set; }

        public int UsuarioId { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public virtual Usuarios.Usuarios usuario {get;set;}
    }
}