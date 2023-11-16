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
        public Task<GenericResponse<DetallePeliculaDTO>> BuscarDetallePelicula(string codigo);

        public Task<GenericResponse<dynamic>> BuscarComentarios(string codigoPelicula);
        public Task<GenericResponse<dynamic>> AgregarComentario(string codigoPelicula, string comentario);
        public Task<GenericResponse<dynamic>> EditarComentario(int id, string comentario);
        public Task<GenericResponse<dynamic>> EliminarComentario(int id);
    }
}