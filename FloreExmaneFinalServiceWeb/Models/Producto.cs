namespace FloreExmaneFinalServiceWeb.Models
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Stock { get; set; }
        public string Categoria { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
    }
}
