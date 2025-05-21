using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using millonarioApi.Models; // Asegúrate de que esta referencia sea correcta para tu DTO
using Millonario.Domain;
using Millonario.Infrastructure;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Millonario.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreguntasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PreguntasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PreguntaDto>>> GetPreguntas()
        {
            if (_context.Preguntas == null)
            {
                return NotFound("No hay preguntas configuradas en la base de datos.");
            }

            var preguntas = await _context.Preguntas
                                          .Include(p => p.Respuestas)
                                          .Select(p => new PreguntaDto
                                          {
                                              Id = p.Id,
                                              Question = p.TextoPregunta,
                                              NivelDificultad = p.NivelDificultad, 
                                              Options = p.Respuestas
                                                             .Select(r => new RespuestaDto
                                                             {
                                                                 Id = r.Id,
                                                                 Text = r.TextoRespuesta,
                                                                 IsCorrect = r.EsCorrecta
                                                             })
                                                             .ToList()
                                          })
                                          .ToListAsync();

            if (!preguntas.Any())
            {
                return NotFound("No se encontraron preguntas en la base de datos.");
            }

            return Ok(preguntas);
        }


        [HttpGet("byDifficulty/{nivel}")]
        public async Task<ActionResult<IEnumerable<PreguntaDto>>> GetPreguntasByDifficulty(int nivel)
        {
            if (_context.Preguntas == null)
            {
                return NotFound("No hay preguntas configuradas en la base de datos.");
            }

            var preguntas = await _context.Preguntas
                                          .Include(p => p.Respuestas)
                                          .Where(p => p.NivelDificultad == nivel)
                                          .Select(p => new PreguntaDto
                                          {
                                              Id = p.Id,
                                              Question = p.TextoPregunta,
                                              NivelDificultad = p.NivelDificultad,
                                              Options = p.Respuestas
                                                             .Select(r => new RespuestaDto
                                                             {
                                                                 Id = r.Id,
                                                                 Text = r.TextoRespuesta,
                                                                 IsCorrect = r.EsCorrecta
                                                             })
                                                             .ToList()
                                          })
                                          .ToListAsync();

            if (!preguntas.Any())
            {
                return NotFound($"No se encontraron preguntas para el nivel de dificultad {nivel}.");
            }

            return Ok(preguntas);
        }
    }
}