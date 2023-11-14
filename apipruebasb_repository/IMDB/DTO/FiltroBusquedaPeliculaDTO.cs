using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apipruebasb_repository.IMDB.DTO
{
    public class FiltroBusquedaPeliculaDTO
    {
        public string? Titulo {get;set;}
        public string? Codigo {get;set;}
        public int? Pagina {get;set;}
    }
}