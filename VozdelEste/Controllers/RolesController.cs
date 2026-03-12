using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using VozdelEste.Models;

namespace VozdelEste.Controllers
{
    public class RolesController : BaseController
    {
        private VozDelEsteDBEntities db = new VozDelEsteDBEntities();

        // GET: Roles
        public ActionResult Index()
        {
            if (!TienePermiso("Ver roles")) return RedirectToAction("Login", "Usuarios");

            return View(db.Roles.ToList());
        }

        // GET: Roles/Details/5
        public ActionResult Details(int? id)
        {
            if (!TienePermiso("Ver roles")) return RedirectToAction("Login", "Usuarios");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var rol = db.Roles.Find(id);
            if (rol == null) return HttpNotFound();

            return View(rol);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            if (!TienePermiso("Crear roles")) return RedirectToAction("Login", "Usuarios");

            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre")] Roles roles)
        {
            if (!TienePermiso("Crear roles")) return RedirectToAction("Login", "Usuarios");

            if (ModelState.IsValid)
            {
                db.Roles.Add(roles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(roles);
        }

        // GET: Roles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TienePermiso("Editar roles")) return RedirectToAction("Login", "Usuarios");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var rol = db.Roles.Find(id);
            if (rol == null) return HttpNotFound();

            var permisosTodos = db.Permisos.ToList();
            var permisosRol = rol.Permisos.Select(p => p.Id).ToList();

            var viewModel = new EditarRolViewModel
            {
                Id = rol.Id,
                Nombre = rol.Nombre,
                PermisosDisponibles = permisosTodos.Select(p => new PermisoCheckbox
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Seleccionado = permisosRol.Contains(p.Id)
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditarRolViewModel model)
        {
            if (!TienePermiso("Editar roles")) return RedirectToAction("Login", "Usuarios");

            if (!ModelState.IsValid)
                return View(model);

            var rol = db.Roles.Include("Permisos").FirstOrDefault(r => r.Id == model.Id);
            if (rol == null) return HttpNotFound();

            rol.Nombre = model.Nombre;
            rol.Permisos.Clear();

            var idsSeleccionados = model.PermisosDisponibles
                .Where(p => p.Seleccionado)
                .Select(p => p.Id)
                .ToList();

            var permisosSeleccionados = db.Permisos
                .Where(p => idsSeleccionados.Contains(p.Id))
                .ToList();

            foreach (var permiso in permisosSeleccionados)
            {
                rol.Permisos.Add(permiso);
            }

            db.Entry(rol).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Roles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!TienePermiso("Eliminar roles")) return RedirectToAction("Login", "Usuarios");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var rol = db.Roles.Find(id);
            if (rol == null) return HttpNotFound();

            return View(rol);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!TienePermiso("Eliminar roles")) return RedirectToAction("Login", "Usuarios");

            var rol = db.Roles.Find(id);
            db.Roles.Remove(rol);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
