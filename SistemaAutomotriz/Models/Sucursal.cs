namespace SistemaAutomotriz.Models
{
    public class Sucursal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Direccion { get; set; } = "";
        public bool Activa { get; set; } = true;

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public ICollection<StockSucursal> Stocks { get; set; } = new List<StockSucursal>();
    }
}
