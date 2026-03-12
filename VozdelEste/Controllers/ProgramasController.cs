using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using VozdelEste.Models;

namespace VozdelEste.Controllers
{
    public class ProgramasController : BaseController
    {
        private VozDelEsteDBEntities db = new VozDelEsteDBEntities();

        // GET: Programas (Público)
        public ActionResult Index()
        {
            return View(db.Programas.ToList());
        }

        // GET: Programas/Details/5 (Público)
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var programa = db.Programas.Find(id);
            if (programa == null) return HttpNotFound();

            return View(programa);
        }

        // GET: Programas/Create (según permiso)
        public ActionResult Create()
        {
            if (!TienePermiso("Crear programas"))
                return RedirectToAction("Login", "Usuarios");

            return View();
        }

        // POST: Programas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nombre,Descripcion,DiaSemana,HoraInicio,HoraFin,Imagen")] Programas programa)
        {
            if (!TienePermiso("Crear programas"))
                return RedirectToAction("Login", "Usuarios");

            if (ModelState.IsValid)
            {
                db.Programas.Add(programa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(programa);
        }

        // GET: Programas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!TienePermiso("Editar programas"))
                return RedirectToAction("Login", "Usuarios");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var programa = db.Programas.Find(id);
            if (programa == null) return HttpNotFound();

            return View(programa);
        }

        // POST: Programas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Imagen,Descripcion,DiaSemana,HoraInicio,HoraFin")] Programas programa)
        {
            if (!TienePermiso("Editar programas"))
                return RedirectToAction("Login", "Usuarios");

            if (ModelState.IsValid)
            {
                db.Entry(programa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(programa);
        }

        // GET: Programas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!TienePermiso("Eliminar programas"))
                return RedirectToAction("Login", "Usuarios");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var programa = db.Programas.Find(id);
            if (programa == null) return HttpNotFound();

            return View(programa);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!TienePermiso("Eliminar programas"))
                return RedirectToAction("Login", "Usuarios");

            var programa = db.Programas.Find(id);

            // 1. Eliminar relaciones con conductores
            var relaciones = db.ProgramaConductores.Where(pc => pc.ProgramaId == id).ToList();
            foreach (var relacion in relaciones)
            {
                db.ProgramaConductores.Remove(relacion);
            }

            // 2. Eliminar el programa
            db.Programas.Remove(programa);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }

        public ActionResult VerConductores(int id)
        {
            using (var db = new VozDelEsteDBEntities())
            {
                var programa = db.Programas.Find(id);
                var conductores = (from pc in db.ProgramaConductores
                                   join c in db.Conductores on pc.ConductorId equals c.Id
                                   where pc.ProgramaId == id
                                   select c).ToList();

                ViewBag.Conductores = conductores;
                ViewBag.ProgramaNombre = programa.Nombre;

                return View();
            }
        }


        public ActionResult AsignarConductores(int id)
        {
            using (var db = new VozDelEsteDBEntities())
            {
                var programa = db.Programas.Find(id);
                var todos = db.Conductores.ToList();
                var asignados = db.ProgramaConductores
                                  .Where(pc => pc.ProgramaId == id)
                                  .Select(pc => pc.ConductorId)
                                  .ToList();

                var model = new AsignarConductoresViewModel
                {
                    ProgramaId = id,
                    ProgramaNombre = programa.Nombre,
                    ConductoresDisponibles = todos.Select(c => new ConductorAsignado
                    {
                        Id = c.Id,
                        Nombre = c.Nombre,
                        Seleccionado = asignados.Contains(c.Id)
                    }).ToList()
                };

                return View(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarConductores(AsignarConductoresViewModel model)
        {
            using (var db = new VozDelEsteDBEntities())
            {
                var existentes = db.ProgramaConductores
                                   .Where(pc => pc.ProgramaId == model.ProgramaId)
                                   .ToList();

                db.ProgramaConductores.RemoveRange(existentes);

                var seleccionados = model.ConductoresDisponibles
                                         .Where(c => c.Seleccionado)
                                         .ToList();

                foreach (var conductor in seleccionados)
                {
                    db.ProgramaConductores.Add(new ProgramaConductores
                    {
                        ProgramaId = model.ProgramaId,
                        ConductorId = conductor.Id
                    });
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        public ActionResult Calendario()
        {
            var programas = db.Programas.ToList(); 
            return View(programas); 
        }

    }
}
