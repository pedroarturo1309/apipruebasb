using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apipruebasb_repository.Authorization;
using apipruebasb_repository.IMDB;
using Microsoft.AspNetCore.Mvc;

namespace apipruebasb.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IMDBController : ControllerBase
    {
        private readonly IIMDBRepository _repository;
        public IMDBController(IIMDBRepository repository)
        {
            _repository = repository;
        }
        [HttpGet("buscar-nombre")]
        public async Task<IActionResult> BuscarPorNombre(string nombre, int pagina)
        {
            var respuesta = await _repository.BuscarPorTitulo(nombre, pagina);
            return Ok(respuesta);
        }

        [HttpGet("buscar-codigo")]
        public async Task<IActionResult> BuscarPorNombre(string codigo)
        {
            var respuesta = await _repository.BuscarDetallePelicula(codigo);
            return Ok(respuesta);
        }

    }
}