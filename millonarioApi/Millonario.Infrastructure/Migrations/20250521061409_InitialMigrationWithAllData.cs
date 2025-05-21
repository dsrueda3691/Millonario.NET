using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Millonario.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrationWithAllData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Preguntas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TextoPregunta = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NivelDificultad = table.Column<int>(type: "int", nullable: false),
                    Categoria = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preguntas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Respuestas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TextoRespuesta = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EsCorrecta = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PreguntaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Respuestas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Respuestas_Preguntas_PreguntaId",
                        column: x => x.PreguntaId,
                        principalTable: "Preguntas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Puntuacion = table.Column<int>(type: "int", nullable: false),
                    FechaRecord = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Records_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Preguntas",
                columns: new[] { "Id", "Categoria", "NivelDificultad", "TextoPregunta" },
                values: new object[,]
                {
                    { 1, "Geografía", 1, "¿Cuál es la capital de Francia?" },
                    { 2, "Astronomía", 1, "¿Qué planeta es conocido como el Planeta Rojo?" },
                    { 3, "Literatura", 2, "¿Quién escribió 'Cien años de soledad'?" },
                    { 4, "Ciencia", 2, "¿Cuál es el elemento químico más abundante en la corteza terrestre?" },
                    { 5, "Historia", 3, "¿En qué año se disolvió la Unión Soviética?" },
                    { 6, "Geografía", 1, "¿Cuál es el río más largo del mundo (considerando solo longitud)?" },
                    { 7, "Arte", 1, "¿Quién pintó la 'Mona Lisa'?" },
                    { 8, "Geografía", 1, "¿Cuál es la capital de Japón?" },
                    { 9, "Matemáticas", 2, "¿Cuántos lados tiene un heptágono?" },
                    { 10, "Biología", 2, "¿Qué animal es el mamífero terrestre más grande?" },
                    { 11, "Economía", 3, "¿Cuál es la moneda oficial de China?" },
                    { 12, "Geografía", 3, "¿Cuál es el país con más población del mundo?" },
                    { 13, "Ciencia", 2, "¿Quién descubrió la penicilina?" },
                    { 14, "Anatomía", 3, "¿Cuál es el hueso más largo del cuerpo humano?" },
                    { 15, "Música", 3, "¿Qué compositor es conocido por la 'Novena Sinfonía'?" }
                });

            migrationBuilder.InsertData(
                table: "Respuestas",
                columns: new[] { "Id", "EsCorrecta", "PreguntaId", "TextoRespuesta" },
                values: new object[,]
                {
                    { 1, true, 1, "París" },
                    { 2, false, 1, "Londres" },
                    { 3, false, 1, "Madrid" },
                    { 4, false, 1, "Berlín" },
                    { 5, false, 2, "Júpiter" },
                    { 6, true, 2, "Marte" },
                    { 7, false, 2, "Venus" },
                    { 8, false, 2, "Saturno" },
                    { 9, true, 6, "Nilo" },
                    { 10, false, 6, "Amazonas" },
                    { 11, false, 6, "Yangtsé" },
                    { 12, false, 6, "Mississippi" },
                    { 13, false, 7, "Vincent van Gogh" },
                    { 14, false, 7, "Pablo Picasso" },
                    { 15, true, 7, "Leonardo da Vinci" },
                    { 16, false, 7, "Claude Monet" },
                    { 17, false, 8, "Pekín" },
                    { 18, false, 8, "Seúl" },
                    { 19, true, 8, "Tokio" },
                    { 20, false, 8, "Bangkok" },
                    { 21, true, 3, "Gabriel García Márquez" },
                    { 22, false, 3, "Mario Vargas Llosa" },
                    { 23, false, 3, "Julio Cortázar" },
                    { 24, false, 3, "Jorge Luis Borges" },
                    { 25, false, 4, "Hierro" },
                    { 26, true, 4, "Oxígeno" },
                    { 27, false, 4, "Silicio" },
                    { 28, false, 4, "Aluminio" },
                    { 29, false, 9, "6" },
                    { 30, true, 9, "7" },
                    { 31, false, 9, "8" },
                    { 32, false, 9, "5" },
                    { 33, false, 10, "Ballena Azul" },
                    { 34, true, 10, "Elefante Africano" },
                    { 35, false, 10, "Jirafa" },
                    { 36, false, 10, "Rinoceronte" },
                    { 37, false, 13, "Marie Curie" },
                    { 38, false, 13, "Louis Pasteur" },
                    { 39, true, 13, "Alexander Fleming" },
                    { 40, false, 13, "Robert Koch" },
                    { 41, false, 5, "1989" },
                    { 42, true, 5, "1991" },
                    { 43, false, 5, "1993" },
                    { 44, false, 5, "1985" },
                    { 45, false, 11, "Yen" },
                    { 46, false, 11, "Won" },
                    { 47, true, 11, "Yuan" },
                    { 48, false, 11, "Rublo" },
                    { 49, false, 12, "Rusia" },
                    { 50, true, 12, "India" },
                    { 51, false, 12, "Estados Unidos" },
                    { 52, false, 12, "China" },
                    { 53, false, 14, "Tibia" },
                    { 54, true, 14, "Fémur" },
                    { 55, false, 14, "Húmero" },
                    { 56, false, 14, "Radio" },
                    { 57, false, 15, "Wolfgang Amadeus Mozart" },
                    { 58, true, 15, "Ludwig van Beethoven" },
                    { 59, false, 15, "Johann Sebastian Bach" },
                    { 60, false, 15, "Richard Wagner" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Records_UsuarioId",
                table: "Records",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Respuestas_PreguntaId",
                table: "Respuestas",
                column: "PreguntaId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Username",
                table: "Usuarios",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Records");

            migrationBuilder.DropTable(
                name: "Respuestas");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Preguntas");
        }
    }
}
