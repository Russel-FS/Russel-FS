using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FloreExmaneFinalServiceWeb.Models;

namespace FloreExmaneFinalServiceWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly string? _connectionString;

        public ProductoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var productos = new List<Producto>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM Producto WHERE Activo = 1", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productos.Add(new Producto
                            {
                                ProductoID = reader.GetInt32(0),
                                Codigo = reader.GetString(1),
                                Nombre = reader.GetString(2),
                                Descripcion = reader.GetString(3),
                                PrecioUnitario = reader.GetDecimal(4),
                                Stock = reader.GetInt32(5),
                                Categoria = reader.GetString(6),
                                FechaCreacion = reader.GetDateTime(7),
                                Activo = reader.GetBoolean(8)
                            });
                        }
                    }
                }
            }
            return Ok(productos);
        }

        [HttpPost]
        public IActionResult Create(Producto producto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "INSERT INTO Producto (Codigo, Nombre, Descripcion, PrecioUnitario, Stock, Categoria) " +
                         "VALUES (@Codigo, @Nombre, @Descripcion, @PrecioUnitario, @Stock, @Categoria)";
                
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Codigo", producto.Codigo);
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@PrecioUnitario", producto.PrecioUnitario);
                    command.Parameters.AddWithValue("@Stock", producto.Stock);
                    command.Parameters.AddWithValue("@Categoria", producto.Categoria);

                    command.ExecuteNonQuery();
                }
            }
            return Ok(new { message = "Producto creado exitosamente" });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Producto producto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "UPDATE Producto SET Codigo = @Codigo, Nombre = @Nombre, Descripcion = @Descripcion, " +
                         "PrecioUnitario = @PrecioUnitario, Stock = @Stock, Categoria = @Categoria " +
                         "WHERE ProductoID = @ProductoID";
                
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ProductoID", id);
                    command.Parameters.AddWithValue("@Codigo", producto.Codigo);
                    command.Parameters.AddWithValue("@Nombre", producto.Nombre);
                    command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@PrecioUnitario", producto.PrecioUnitario);
                    command.Parameters.AddWithValue("@Stock", producto.Stock);
                    command.Parameters.AddWithValue("@Categoria", producto.Categoria);

                    var rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                        return NotFound(new { message = "Producto no encontrado" });
                }
            }
            return Ok(new { message = "Producto actualizado exitosamente" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "UPDATE Producto SET Activo = 0 WHERE ProductoID = @ProductoID";
                
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ProductoID", id);
                    var rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                        return NotFound(new { message = "Producto no encontrado" });
                }
            }
            return Ok(new { message = "Producto eliminado exitosamente" });
        }
    }
}
