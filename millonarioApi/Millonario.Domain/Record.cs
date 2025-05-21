using System;

namespace Millonario.Domain
{
    public class Record
    {
        public int Id { get; set; } 
        public int Puntuacion { get; set; } 
        public DateTime FechaRecord { get; set; }


        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}