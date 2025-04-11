namespace FloreExmaneFinalServiceWeb.Models
{
    public class DetalleOrdenCompra
    {
        public int DetalleID { get; set; }
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
