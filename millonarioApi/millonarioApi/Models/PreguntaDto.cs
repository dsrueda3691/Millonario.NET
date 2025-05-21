

namespace millonarioApi.Models 
{
   
    public class PreguntaDto
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public int NivelDificultad { get; set; } 
        public List<RespuestaDto> Options { get; set; } = new List<RespuestaDto>();
    }


    public class RespuestaDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}