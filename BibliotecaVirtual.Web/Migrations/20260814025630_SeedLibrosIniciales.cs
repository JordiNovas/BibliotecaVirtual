using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BibliotecaVirtual.Web.Migrations
{
    /// <inheritdoc />
    public partial class SeedLibrosIniciales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Libros",
                columns: new[] { "Id", "Autor", "Categoria", "Disponible", "ISBN", "Titulo" },
                values: new object[,]
                {
                    { 1, "Gabriel García Márquez", "Novela", true, "978-0307474728", "Cien años de soledad" },
                    { 2, "Antoine de Saint-Exupéry", "Infantil", true, "978-0156013987", "El principito" },
                    { 3, "Robert C. Martin", "Tecnología", true, "978-0132350884", "Clean Code" },
                    { 4, "Miguel de Cervantes", "Clásico", true, "978-8424115463", "Don Quijote de la Mancha" },
                    { 5, "Bradley L. Jones", "Tecnología", true, "978-0672320712", "Aprende C# en 21 Días" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
