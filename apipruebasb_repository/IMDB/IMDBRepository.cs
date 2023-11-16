using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apipruebasb_entities.Peliculas;
using apipruebasb_repository.IMDB.DTO;
using apipruebasb_repository.Usuario;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace apipruebasb_repository.IMDB
{
    public class IMDBRepository : IIMDBRepository
    {
        private readonly IConfiguration _configuration;
        private readonly PruebasbDBContext _context;
        public IMDBRepository(IConfiguration configuration, PruebasbDBContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task<GenericResponse<ListaPeliculaDTO>> BuscarPorTitulo(string Titulo, int pagina = 1)
        {
            GenericResponse<ListaPeliculaDTO> respuesta = new GenericResponse<ListaPeliculaDTO>();
            string? json = await EnviarRequest(TipoBusqueda.Lista, Titulo, pagina);

            if (!string.IsNullOrEmpty(json))
                respuesta.Data = JsonConvert.DeserializeObject<ListaPeliculaDTO>(json);
            else
                respuesta.AddNotification("Ha ocurrido un error consultado las peliculas");

            return respuesta;
        }

        private enum TipoBusqueda
        {
            Lista,
            Detalle
        }
        private async Task<string?> EnviarRequest(TipoBusqueda tipo, string? titulo, int? pagina, string? codigo = null)
        {
            string apiKey = _configuration["IMDB:key"];
            string baseurl = _configuration["IMDB:url"];

            using (HttpClient client = new HttpClient())
            {
                string? query = "";

                if (tipo == TipoBusqueda.Lista)
                    query = $"&s={titulo}&page={pagina}";
                else
                    query = $"&i={codigo}";

                string apiUrl = $"{baseurl}?apikey={apiKey}{query}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    return jsonResponse;
                }

                return null;
            }
        }

        public async Task<GenericResponse<DetallePeliculaDTO>> BuscarDetallePelicula(string codigo)
        {
            GenericResponse<DetallePeliculaDTO> respuesta = new GenericResponse<DetallePeliculaDTO>();
            string? json = await EnviarRequest(TipoBusqueda.Detalle, null, null, codigo);

            if (!string.IsNullOrEmpty(json))
                respuesta.Data = JsonConvert.DeserializeObject<DetallePeliculaDTO>(json);
            else
                respuesta.AddNotification("Ha ocurrido un error consultado las peliculas");

            return respuesta;
        }

        public async Task<GenericResponse<dynamic>> BuscarComentarios(string codigoPelicula)
        {
            GenericResponse<dynamic> respuesta = new GenericResponse<dynamic>();
            try
            {
                respuesta.Data = await _context.ComentariosPeliculas.Where(x => x.PeliculaId == codigoPelicula)
                                                   .Select(pelicula => new
                                                   {
                                                       puedeEditar = pelicula.UsuarioId == CurrentUser.UsuarioId,
                                                       pelicula.Id,
                                                       pelicula.Comentario,
                                                       pelicula.FechaCreacion,
                                                       NombreUsuario = $"{pelicula.usuario.Nombres} {pelicula.usuario.Apellidos}"
                                                   }).ToListAsync();
            }
            catch
            {
                respuesta.AddNotification("Hubo un error buscando los comentarios.");
            }


            return respuesta;
        }

        public async Task<GenericResponse<dynamic>> AgregarComentario(string codigoPelicula, string comentario)
        {
            GenericResponse<dynamic> respuesta = new GenericResponse<dynamic>();
            try
            {
                await _context.ComentariosPeliculas.AddAsync(new ComentariosPeliculas()
                {
                    Comentario = comentario,
                    PeliculaId = codigoPelicula,
                    FechaCreacion = DateTime.Now,
                    UsuarioId = CurrentUser.UsuarioId
                });

                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                respuesta.AddNotification("Hubo un error al guardar el comentario.");
            }
            return respuesta;
        }

        public async Task<GenericResponse<dynamic>> EditarComentario(int id, string comentario)
        {
            var item = _context.ComentariosPeliculas.FirstOrDefault(x => x.Id == id && x.UsuarioId == CurrentUser.UsuarioId);

            if(item != null) {
                item.Comentario = comentario;
                await _context.SaveChangesAsync();
            }
            return new GenericResponse<dynamic>();

        }

        public async Task<GenericResponse<dynamic>> EliminarComentario(int id)
        {
            var comentario = _context.ComentariosPeliculas.FirstOrDefault(x => x.Id == id && x.UsuarioId == CurrentUser.UsuarioId);

            if(comentario != null) {
                comentario.Disponible = false;
                await _context.SaveChangesAsync();
            }

            return new GenericResponse<dynamic>();
        }
    }
}