using SistemaAutomotriz.Servicios.DA;

namespace SistemaAutomotriz.Servicios
{
    public class DataAccessService
    {
        private readonly IServiceProvider _serviceProvider;

        public DataAccessService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public UsuarioDA Usuario => _serviceProvider.GetRequiredService<UsuarioDA>();
        public SucursalDA Sucursal => _serviceProvider.GetRequiredService<SucursalDA>();
        public PanelDA Panel => _serviceProvider.GetRequiredService<PanelDA>();
    }
}
