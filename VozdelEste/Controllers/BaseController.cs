using System.Web.Mvc;
using VozdelEste.Models;

namespace VozdelEste.Controllers
{
    public class BaseController : Controller
    {
        protected Usuarios UsuarioActual => Session["Usuario"] as Usuarios;

        protected bool TienePermiso(string nombre)
        {
            return UsuarioActual != null && UsuarioActual.TienePermiso(nombre);
        }

        protected ActionResult RedirigirSiNoTienePermiso(string permiso)
        {
            if (!TienePermiso(permiso))
                return RedirectToAction("Login", "Usuarios");

            return null;
        }
    }
}
