using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using FloreExmaneFinalServiceWeb.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using ClosedXML.Excel;

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

        [HttpGet("reporte-pdf/{id}")]
        public IActionResult GenerarReportePDF(int id)
        {
            var reporte = ObtenerDatosReporte(id);
            if (reporte == null)
                return NotFound(new { message = "Producto no encontrado" });

            using (var memoryStream = new MemoryStream())
            {
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                document.Add(new Paragraph($"Reporte de Producto - {reporte.Nombre}"));
                document.Add(new Paragraph($"Código: {reporte.Codigo}"));
                document.Add(new Paragraph($"Precio Unitario: {reporte.PrecioUnitario:C}"));
                document.Add(new Paragraph($"Stock Actual: {reporte.StockActual}"));
                document.Add(new Paragraph($"Cantidad Vendida: {reporte.CantidadVendida}"));
                document.Add(new Paragraph($"Total Ventas: {reporte.TotalVentas:C}"));

                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", $"ReporteProducto_{id}.pdf");
            }
        }

        [HttpGet("reporte-excel/{id}")]
        public IActionResult GenerarReporteExcel(int id)
        {
            var reporte = ObtenerDatosReporte(id);
            if (reporte == null)
                return NotFound(new { message = "Producto no encontrado" });

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reporte Producto");
                
                worksheet.Cell("A1").Value = "Código";
                worksheet.Cell("B1").Value = "Nombre";
                worksheet.Cell("C1").Value = "Precio Unitario";
                worksheet.Cell("D1").Value = "Stock Actual";
                worksheet.Cell("E1").Value = "Cantidad Vendida";
                worksheet.Cell("F1").Value = "Total Ventas";

                worksheet.Cell("A2").Value = reporte.Codigo;
                worksheet.Cell("B2").Value = reporte.Nombre;
                worksheet.Cell("C2").Value = reporte.PrecioUnitario;
                worksheet.Cell("D2").Value = reporte.StockActual;
                worksheet.Cell("E2").Value = reporte.CantidadVendida;
                worksheet.Cell("F2").Value = reporte.TotalVentas;

                using (var memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    return File(memoryStream.ToArray(), 
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"ReporteProducto_{id}.xlsx");
                }
            }
        }

        private ReporteProductoDTO ObtenerDatosReporte(int id)
        {
            ReporteProductoDTO reporte = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @"
                    SELECT 
                        p.Codigo,
                        p.Nombre,
                        p.PrecioUnitario,
                        p.Stock as StockActual,
                        ISNULL(SUM(d.Cantidad), 0) as CantidadVendida,
                        ISNULL(SUM(d.Subtotal), 0) as TotalVentas
                    FROM Producto p
                    LEFT JOIN DetalleOrdenCompra d ON p.ProductoID = d.ProductoID
                    WHERE p.ProductoID = @ProductoID AND p.Activo = 1
                    GROUP BY p.Codigo, p.Nombre, p.PrecioUnitario, p.Stock";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ProductoID", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            reporte = new ReporteProductoDTO
                            {
                                Codigo = reader.GetString(0),
                                Nombre = reader.GetString(1),
                                PrecioUnitario = reader.GetDecimal(2),
                                StockActual = reader.GetInt32(3),
                                CantidadVendida = reader.GetInt32(4),
                                TotalVentas = reader.GetDecimal(5)
                            };
                        }
                    }
                }
            }
            return reporte;
        }
    }
}
