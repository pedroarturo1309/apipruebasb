using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apipruebasb_repository.Authorization;
using apipruebasb_repository.IMDB;
using apipruebasb_repository.IMDB.DTO;
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

        [HttpPost("guardar-comentario")]
        public async Task<IActionResult> GuardarComentario([FromBody] GuardarComentarioDTO model)
        {
            var respuesta = await _repository.AgregarComentario(model.CodigoPelicula, model.Comentario);
            return Ok(respuesta);
        }

        [HttpPut("editar-comentario")]
        public async Task<IActionResult> EditarComentario([FromBody] GuardarComentarioDTO model)
        {
            var respuesta = await _repository.EditarComentario(model.Id, model.Comentario);
            return Ok(respuesta);
        }

        [HttpDelete("eliminar-comentario")]
        public async Task<IActionResult> EditarComentario(int id)
        {
            var respuesta = await _repository.EliminarComentario(id);
            return Ok(respuesta);
        }

        [HttpGet("buscar-comentarios")]
        public async Task<IActionResult> BuscarComentarios(string codigo)
        {
            var respuesta = await _repository.BuscarComentarios(codigo);
            return Ok(respuesta);
        }
    }
}