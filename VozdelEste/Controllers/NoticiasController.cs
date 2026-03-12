using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using VozdelEste.Models;

namespace VozdelEste.Controllers
{
    public class NoticiasController : BaseController
    {
        private VozDelEsteDBEntities db = new VozDelEsteDBEntities();

        public ActionResult Index()
        {
            return View(db.Noticias.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var noticia = db.Noticias.Find(id);
            if (noticia == null) return HttpNotFound();
            return View(noticia);
        }

        public ActionResult Create()
        {
            if (!TienePermiso("Crear noticias")) return RedirectToAction("Login", "Usuarios");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Titulo,Contenido,FechaPublicacion,Imagen")] Noticias noticia)
        {
            if (!TienePermiso("Crear noticias")) return RedirectToAction("Login", "Usuarios");

            if (ModelState.IsValid)
            {
                db.Noticias.Add(noticia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(noticia);
        }

        public ActionResult Edit(int? id)
        {
            if (!TienePermiso("Editar noticias")) return RedirectToAction("Login", "Usuarios");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var noticia = db.Noticias.Find(id);
            if (noticia == null) return HttpNotFound();
            return View(noticia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Titulo,Contenido,FechaPublicacion,Imagen")] Noticias noticia)
        {
            if (!TienePermiso("Editar noticias")) return RedirectToAction("Login", "Usuarios");

            if (ModelState.IsValid)
            {
                db.Entry(noticia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(noticia);
        }

        public ActionResult Delete(int? id)
        {
            if (!TienePermiso("Eliminar noticias")) return RedirectToAction("Login", "Usuarios");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var noticia = db.Noticias.Find(id);
            if (noticia == null) return HttpNotFound();
            return View(noticia);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!TienePermiso("Eliminar noticias")) return RedirectToAction("Login", "Usuarios");

            var noticia = db.Noticias.Find(id);
            db.Noticias.Remove(noticia);
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
