using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apipruebasb_repository.IMDB.DTO
{
    public class ListaPeliculaDTO
    {
        public List<MovieDto> Search { get; set; }
        public string TotalResults { get; set; }
        public string Response { get; set; }
    }

    public class MovieDto
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string ImdbID { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }
    }
}
