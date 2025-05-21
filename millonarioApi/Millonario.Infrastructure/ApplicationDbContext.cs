using Microsoft.EntityFrameworkCore;
using Millonario.Domain;
using System.Collections.Generic;

namespace Millonario.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Pregunta> Preguntas { get; set; }
        public DbSet<Respuesta> Respuestas { get; set; }
        public DbSet<Record> Records { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Respuesta>()
                .HasOne(r => r.Pregunta)
                .WithMany(p => p.Respuestas)
                .HasForeignKey(r => r.PreguntaId);

            modelBuilder.Entity<Record>()
                .HasOne(r => r.Usuario)
                .WithMany(u => u.Records)
                .HasForeignKey(r => r.UsuarioId);

            // *** PREGUNTAS *** (Ajustadas para 5 de cada dificultad)
            modelBuilder.Entity<Pregunta>().HasData(
                // --- Dificultad 1 (Necesitas 5 preguntas) ---
                new Pregunta { Id = 1, TextoPregunta = "¿Cuál es la capital de Francia?", NivelDificultad = 1, Categoria = "Geografía" },
                new Pregunta { Id = 2, TextoPregunta = "¿Qué planeta es conocido como el Planeta Rojo?", NivelDificultad = 1, Categoria = "Astronomía" },
                new Pregunta { Id = 6, TextoPregunta = "¿Cuál es el río más largo del mundo (considerando solo longitud)?", NivelDificultad = 1, Categoria = "Geografía" }, // Nilo es la respuesta más aceptada
                new Pregunta { Id = 7, TextoPregunta = "¿Quién pintó la 'Mona Lisa'?", NivelDificultad = 1, Categoria = "Arte" },
                new Pregunta { Id = 8, TextoPregunta = "¿Cuál es la capital de Japón?", NivelDificultad = 1, Categoria = "Geografía" },

                // --- Dificultad 2 (Necesitas 5 preguntas) ---
                new Pregunta { Id = 3, TextoPregunta = "¿Quién escribió 'Cien años de soledad'?", NivelDificultad = 2, Categoria = "Literatura" },
                new Pregunta { Id = 4, TextoPregunta = "¿Cuál es el elemento químico más abundante en la corteza terrestre?", NivelDificultad = 2, Categoria = "Ciencia" },
                new Pregunta { Id = 9, TextoPregunta = "¿Cuántos lados tiene un heptágono?", NivelDificultad = 2, Categoria = "Matemáticas" },
                new Pregunta { Id = 10, TextoPregunta = "¿Qué animal es el mamífero terrestre más grande?", NivelDificultad = 2, Categoria = "Biología" },
                new Pregunta { Id = 13, TextoPregunta = "¿Quién descubrió la penicilina?", NivelDificultad = 2, Categoria = "Ciencia" },

                // --- Dificultad 3 (Necesitas 5 preguntas) ---
                new Pregunta { Id = 5, TextoPregunta = "¿En qué año se disolvió la Unión Soviética?", NivelDificultad = 3, Categoria = "Historia" },
                new Pregunta { Id = 11, TextoPregunta = "¿Cuál es la moneda oficial de China?", NivelDificultad = 3, Categoria = "Economía" }, // Modificado de Reino Unido
                new Pregunta { Id = 12, TextoPregunta = "¿Cuál es el país con más población del mundo?", NivelDificultad = 3, Categoria = "Geografía" }, // Modificado de Rusia
                new Pregunta { Id = 14, TextoPregunta = "¿Cuál es el hueso más largo del cuerpo humano?", NivelDificultad = 3, Categoria = "Anatomía" }, // Modificado de Sahara
                new Pregunta { Id = 15, TextoPregunta = "¿Qué compositor es conocido por la 'Novena Sinfonía'?", NivelDificultad = 3, Categoria = "Música" } // Nueva pregunta
            );

            // *** RESPUESTAS *** (Ajustadas para nuevas preguntas y IDs)
            modelBuilder.Entity<Respuesta>().HasData(
                // Pregunta 1: ¿Cuál es la capital de Francia? (D1)
                new Respuesta { Id = 1, PreguntaId = 1, TextoRespuesta = "París", EsCorrecta = true },
                new Respuesta { Id = 2, PreguntaId = 1, TextoRespuesta = "Londres", EsCorrecta = false },
                new Respuesta { Id = 3, PreguntaId = 1, TextoRespuesta = "Madrid", EsCorrecta = false },
                new Respuesta { Id = 4, PreguntaId = 1, TextoRespuesta = "Berlín", EsCorrecta = false },

                // Pregunta 2: ¿Qué planeta es conocido como el Planeta Rojo? (D1)
                new Respuesta { Id = 5, PreguntaId = 2, TextoRespuesta = "Júpiter", EsCorrecta = false },
                new Respuesta { Id = 6, PreguntaId = 2, TextoRespuesta = "Marte", EsCorrecta = true },
                new Respuesta { Id = 7, PreguntaId = 2, TextoRespuesta = "Venus", EsCorrecta = false },
                new Respuesta { Id = 8, PreguntaId = 2, TextoRespuesta = "Saturno", EsCorrecta = false },

                // Pregunta 6: ¿Cuál es el río más largo del mundo (considerando solo longitud)? (D1)
                new Respuesta { Id = 9, PreguntaId = 6, TextoRespuesta = "Nilo", EsCorrecta = true },
                new Respuesta { Id = 10, PreguntaId = 6, TextoRespuesta = "Amazonas", EsCorrecta = false },
                new Respuesta { Id = 11, PreguntaId = 6, TextoRespuesta = "Yangtsé", EsCorrecta = false },
                new Respuesta { Id = 12, PreguntaId = 6, TextoRespuesta = "Mississippi", EsCorrecta = false },

                // Pregunta 7: ¿Quién pintó la 'Mona Lisa'? (D1)
                new Respuesta { Id = 13, PreguntaId = 7, TextoRespuesta = "Vincent van Gogh", EsCorrecta = false },
                new Respuesta { Id = 14, PreguntaId = 7, TextoRespuesta = "Pablo Picasso", EsCorrecta = false },
                new Respuesta { Id = 15, PreguntaId = 7, TextoRespuesta = "Leonardo da Vinci", EsCorrecta = true },
                new Respuesta { Id = 16, PreguntaId = 7, TextoRespuesta = "Claude Monet", EsCorrecta = false },

                // Pregunta 8: ¿Cuál es la capital de Japón? (D1)
                new Respuesta { Id = 17, PreguntaId = 8, TextoRespuesta = "Pekín", EsCorrecta = false },
                new Respuesta { Id = 18, PreguntaId = 8, TextoRespuesta = "Seúl", EsCorrecta = false },
                new Respuesta { Id = 19, PreguntaId = 8, TextoRespuesta = "Tokio", EsCorrecta = true },
                new Respuesta { Id = 20, PreguntaId = 8, TextoRespuesta = "Bangkok", EsCorrecta = false },

                // Pregunta 3: ¿Quién escribió 'Cien años de soledad'? (D2)
                new Respuesta { Id = 21, PreguntaId = 3, TextoRespuesta = "Gabriel García Márquez", EsCorrecta = true },
                new Respuesta { Id = 22, PreguntaId = 3, TextoRespuesta = "Mario Vargas Llosa", EsCorrecta = false },
                new Respuesta { Id = 23, PreguntaId = 3, TextoRespuesta = "Julio Cortázar", EsCorrecta = false },
                new Respuesta { Id = 24, PreguntaId = 3, TextoRespuesta = "Jorge Luis Borges", EsCorrecta = false },

                // Pregunta 4: ¿Cuál es el elemento químico más abundante en la corteza terrestre? (D2)
                new Respuesta { Id = 25, PreguntaId = 4, TextoRespuesta = "Hierro", EsCorrecta = false },
                new Respuesta { Id = 26, PreguntaId = 4, TextoRespuesta = "Oxígeno", EsCorrecta = true },
                new Respuesta { Id = 27, PreguntaId = 4, TextoRespuesta = "Silicio", EsCorrecta = false },
                new Respuesta { Id = 28, PreguntaId = 4, TextoRespuesta = "Aluminio", EsCorrecta = false },

                // Pregunta 9: ¿Cuántos lados tiene un heptágono? (D2)
                new Respuesta { Id = 29, PreguntaId = 9, TextoRespuesta = "6", EsCorrecta = false },
                new Respuesta { Id = 30, PreguntaId = 9, TextoRespuesta = "7", EsCorrecta = true },
                new Respuesta { Id = 31, PreguntaId = 9, TextoRespuesta = "8", EsCorrecta = false },
                new Respuesta { Id = 32, PreguntaId = 9, TextoRespuesta = "5", EsCorrecta = false },

                // Pregunta 10: ¿Qué animal es el mamífero terrestre más grande? (D2)
                new Respuesta { Id = 33, PreguntaId = 10, TextoRespuesta = "Ballena Azul", EsCorrecta = false },
                new Respuesta { Id = 34, PreguntaId = 10, TextoRespuesta = "Elefante Africano", EsCorrecta = true },
                new Respuesta { Id = 35, PreguntaId = 10, TextoRespuesta = "Jirafa", EsCorrecta = false },
                new Respuesta { Id = 36, PreguntaId = 10, TextoRespuesta = "Rinoceronte", EsCorrecta = false },

                // Pregunta 13: ¿Quién descubrió la penicilina? (D2)
                new Respuesta { Id = 37, PreguntaId = 13, TextoRespuesta = "Marie Curie", EsCorrecta = false },
                new Respuesta { Id = 38, PreguntaId = 13, TextoRespuesta = "Louis Pasteur", EsCorrecta = false },
                new Respuesta { Id = 39, PreguntaId = 13, TextoRespuesta = "Alexander Fleming", EsCorrecta = true },
                new Respuesta { Id = 40, PreguntaId = 13, TextoRespuesta = "Robert Koch", EsCorrecta = false },

                // Pregunta 5: ¿En qué año se disolvió la Unión Soviética? (D3)
                new Respuesta { Id = 41, PreguntaId = 5, TextoRespuesta = "1989", EsCorrecta = false },
                new Respuesta { Id = 42, PreguntaId = 5, TextoRespuesta = "1991", EsCorrecta = true },
                new Respuesta { Id = 43, PreguntaId = 5, TextoRespuesta = "1993", EsCorrecta = false },
                new Respuesta { Id = 44, PreguntaId = 5, TextoRespuesta = "1985", EsCorrecta = false },

                // Pregunta 11: ¿Cuál es la moneda oficial de China? (D3)
                new Respuesta { Id = 45, PreguntaId = 11, TextoRespuesta = "Yen", EsCorrecta = false },
                new Respuesta { Id = 46, PreguntaId = 11, TextoRespuesta = "Won", EsCorrecta = false },
                new Respuesta { Id = 47, PreguntaId = 11, TextoRespuesta = "Yuan", EsCorrecta = true },
                new Respuesta { Id = 48, PreguntaId = 11, TextoRespuesta = "Rublo", EsCorrecta = false },

                // Pregunta 12: ¿Cuál es el país con más población del mundo? (D3)
                new Respuesta { Id = 49, PreguntaId = 12, TextoRespuesta = "Rusia", EsCorrecta = false },
                new Respuesta { Id = 50, PreguntaId = 12, TextoRespuesta = "India", EsCorrecta = true }, // Ahora India tiene más población
                new Respuesta { Id = 51, PreguntaId = 12, TextoRespuesta = "Estados Unidos", EsCorrecta = false },
                new Respuesta { Id = 52, PreguntaId = 12, TextoRespuesta = "China", EsCorrecta = false },

                // Pregunta 14: ¿Cuál es el hueso más largo del cuerpo humano? (D3)
                new Respuesta { Id = 53, PreguntaId = 14, TextoRespuesta = "Tibia", EsCorrecta = false },
                new Respuesta { Id = 54, PreguntaId = 14, TextoRespuesta = "Fémur", EsCorrecta = true },
                new Respuesta { Id = 55, PreguntaId = 14, TextoRespuesta = "Húmero", EsCorrecta = false },
                new Respuesta { Id = 56, PreguntaId = 14, TextoRespuesta = "Radio", EsCorrecta = false },

                // Pregunta 15: ¿Qué compositor es conocido por la 'Novena Sinfonía'? (D3)
                new Respuesta { Id = 57, PreguntaId = 15, TextoRespuesta = "Wolfgang Amadeus Mozart", EsCorrecta = false },
                new Respuesta { Id = 58, PreguntaId = 15, TextoRespuesta = "Ludwig van Beethoven", EsCorrecta = true },
                new Respuesta { Id = 59, PreguntaId = 15, TextoRespuesta = "Johann Sebastian Bach", EsCorrecta = false },
                new Respuesta { Id = 60, PreguntaId = 15, TextoRespuesta = "Richard Wagner", EsCorrecta = false }
            );
        }
    }
}