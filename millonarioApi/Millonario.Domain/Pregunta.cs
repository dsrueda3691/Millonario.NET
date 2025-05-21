using System.Collections.Generic;

namespace Millonario.Domain
{
    public class Pregunta
    {
        public int Id { get; set; } 
        public string TextoPregunta { get; set; } 
        public int NivelDificultad { get; set; } 
        public string Categoria { get; set; } 


        public ICollection<Respuesta> Respuestas { get; set; } = new List<Respuesta>();
    }
}