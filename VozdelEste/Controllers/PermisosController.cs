using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using VozdelEste.Models;

namespace VozdelEste.Controllers
{
    public class PermisosController : BaseController
    {
        private VozDelEsteDBEntities db = new VozDelEsteDBEntities();


        public ActionResult Index()
        {
            if (!TienePermiso("Ver permisos"))
                return RedirectToAction("Login", "Usuarios");

            return View(db.Permisos.ToList());
        }

        public ActionResult Create()
        {
            if (!TienePermiso("Crear permisos"))
                return RedirectToAction("Login", "Usuarios");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nombre")] Permisos permiso)
        {
            if (!TienePermiso("Crear permisos"))
                return RedirectToAction("Login", "Usuarios");

            if (ModelState.IsValid)
            {
                db.Permisos.Add(permiso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(permiso);
        }

        public ActionResult Details(int? id)
        {
            if (!TienePermiso("Ver permisos"))
                return RedirectToAction("Login", "Usuarios");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var permiso = db.Permisos.Find(id);
            if (permiso == null) return HttpNotFound();

            return View(permiso);
        }

        public ActionResult Edit(int? id)
        {
            if (!TienePermiso("Editar permisos"))
                return RedirectToAction("Login", "Usuarios");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var permiso = db.Permisos.Find(id);
            if (permiso == null) return HttpNotFound();

            return View(permiso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre")] Permisos permiso)
        {
            if (!TienePermiso("Editar permisos"))
                return RedirectToAction("Login", "Usuarios");

            if (ModelState.IsValid)
            {
                db.Entry(permiso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(permiso);
        }

        public ActionResult Delete(int? id)
        {
            if (!TienePermiso("Eliminar permisos"))
                return RedirectToAction("Login", "Usuarios");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var permiso = db.Permisos.Find(id);
            if (permiso == null) return HttpNotFound();

            return View(permiso);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!TienePermiso("Eliminar permisos"))
                return RedirectToAction("Login", "Usuarios");

            var permiso = db.Permisos.Find(id);
            db.Permisos.Remove(permiso);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
