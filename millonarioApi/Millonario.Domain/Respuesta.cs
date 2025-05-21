namespace Millonario.Domain
{
    public class Respuesta
    {
        public int Id { get; set; } 
        public string TextoRespuesta { get; set; } 
        public bool EsCorrecta { get; set; } 

        public int PreguntaId { get; set; }
       
        public Pregunta Pregunta { get; set; }
    }
}