using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apipruebasb_repository.IMDB;
using Microsoft.AspNetCore.Mvc;

namespace apipruebasb.Controllers
{
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
            var respuesta = await _repository.BuscarPorTitulo(nombre);
            return Ok(respuesta);
        }

    }
}