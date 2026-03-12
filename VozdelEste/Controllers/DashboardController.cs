using System.Linq;
using System.Web.Mvc;
using VozdelEste.Models;

namespace VozdelEste.Controllers
{
    public class DashboardController : BaseController
    {
        private VozDelEsteDBEntities db = new VozDelEsteDBEntities();

        public ActionResult Index()
        {
            if (!TienePermiso("Ver dashboard"))
                return RedirectToAction("Login", "Usuarios");

            ViewBag.TotalUsuarios = db.Usuarios.Count();
            ViewBag.TotalProgramas = db.Programas.Count();
            ViewBag.TotalConductores = db.Conductores.Count();
            ViewBag.TotalPatrocinadores = db.Patrocinadores.Count();
            ViewBag.TotalNoticias = db.Noticias.Count();
            ViewBag.TotalRoles = db.Roles.Count();

            var ultimoPrograma = db.Programas.OrderByDescending(p => p.Id).FirstOrDefault();
            ViewBag.UltimoProgramaId = ultimoPrograma?.Id;

            return View();
        }
    }
}
