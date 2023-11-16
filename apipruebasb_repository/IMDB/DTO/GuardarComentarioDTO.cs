using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apipruebasb_repository.IMDB.DTO
{
    public class GuardarComentarioDTO
    {
        public int Id { get; set; }
        public string? CodigoPelicula { get; set; }
        public string? Comentario { get; set; }
    }
}