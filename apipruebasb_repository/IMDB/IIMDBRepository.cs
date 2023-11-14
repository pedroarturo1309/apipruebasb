using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apipruebasb_repository.IMDB.DTO;

namespace apipruebasb_repository.IMDB
{
    public interface IIMDBRepository
    {
        public Task<GenericResponse<ListaPeliculaDTO>> BuscarPorTitulo(string Titulo, int pagina = 1);
    }
}