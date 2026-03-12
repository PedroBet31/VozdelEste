using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VozdelEste.Models;

namespace VozdelEste.Controllers
{
    public class PatrocinadoresController : BaseController
    {
        private VozDelEsteDBEntities db = new VozDelEsteDBEntities();

        public ActionResult Index()
        {
            if (!TienePermiso("Ver patrocinadores"))
                return RedirectToAction("Login", "Usuarios");

            return View(db.Patrocinadores.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (!TienePermiso("Ver patrocinadores")) return RedirectToAction("Login", "Usuarios");
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var patrocinador = db.Patrocinadores.Find(id);
            if (patrocinador == null) return HttpNotFound();

            return View(patrocinador);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (!TienePermiso("Crear patrocinadores"))
                return RedirectToAction("Login", "Usuarios");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Imagen")] Patrocinadores patrocinador, HttpPostedFileBase imagen)
        {
            if (!TienePermiso("Crear patrocinadores"))
                return RedirectToAction("Login", "Usuarios");

            if (imagen != null && imagen.ContentLength > 0)
            {
                using (var reader = new BinaryReader(imagen.InputStream))
                {
                    patrocinador.Imagen = reader.ReadBytes(imagen.ContentLength);
                }
            }

            if (ModelState.IsValid)
            {
                db.Patrocinadores.Add(patrocinador);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(patrocinador);
        }



        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!TienePermiso("Editar patrocinadores"))
                return RedirectToAction("Login", "Usuarios");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var patrocinador = db.Patrocinadores.Find(id);
            if (patrocinador == null) return HttpNotFound();

            return View(patrocinador);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude = "Imagen")] Patrocinadores patrocinador, HttpPostedFileBase imagen)
        {
            if (!TienePermiso("Editar patrocinadores"))
                return RedirectToAction("Login", "Usuarios");

            var existente = db.Patrocinadores.Find(patrocinador.Id);
            if (existente == null) return HttpNotFound();

            existente.Nombre = patrocinador.Nombre;
            existente.Descripcion = patrocinador.Descripcion;
            existente.PlanDiario = patrocinador.PlanDiario;

            if (imagen != null && imagen.ContentLength > 0)
            {
                using (var reader = new BinaryReader(imagen.InputStream))
                {
                    existente.Imagen = reader.ReadBytes(imagen.ContentLength);
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Delete(int? id)
        {
            if (!TienePermiso("Eliminar patrocinadores")) return RedirectToAction("Login", "Usuarios");
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var patrocinador = db.Patrocinadores.Find(id);
            if (patrocinador == null) return HttpNotFound();

            return View(patrocinador);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!TienePermiso("Eliminar patrocinadores")) return RedirectToAction("Login", "Usuarios");

            var patrocinador = db.Patrocinadores.Find(id);
            db.Patrocinadores.Remove(patrocinador);
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
