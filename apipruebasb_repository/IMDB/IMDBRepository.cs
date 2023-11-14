using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apipruebasb_repository.IMDB.DTO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace apipruebasb_repository.IMDB
{
    public class IMDBRepository : IIMDBRepository
    {
        private readonly IConfiguration _configuration;
        public IMDBRepository(IConfiguration configuration)
        {
            _configuration = configuration;
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
    }
}