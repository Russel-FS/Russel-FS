namespace FloreExmaneFinalServiceWeb.Models
{
    public class ReporteProductoDTO
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int StockActual { get; set; }
        public int CantidadVendida { get; set; }
        public decimal TotalVentas { get; set; }
    }
}
