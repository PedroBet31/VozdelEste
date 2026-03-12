using System.Linq;

namespace VozdelEste.Models
{
    public partial class Usuarios
    {
        public bool TienePermiso(string nombrePermiso)
        {
            return this.Roles != null &&
                   this.Roles.Permisos != null &&
                   this.Roles.Permisos.Any(p => p.Nombre == nombrePermiso);
        }
    }
}
