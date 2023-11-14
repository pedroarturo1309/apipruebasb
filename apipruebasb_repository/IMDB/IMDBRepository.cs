using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                                                       NombreUsuario = $"{pelicula.usuario.Nombres} {pelicula.usuario.Apellidos}"
                                                   }).ToListAsync();
            }
            catch
            {
                respuesta.AddNotification("Hubo un error buscando los comentarios.");
            }


            return respuesta;
        }

        public Task<GenericResponse<dynamic>> AgregarComentario(string codigoPelicula, string comentario)
        {
            throw new NotImplementedException();
        }
    }
}