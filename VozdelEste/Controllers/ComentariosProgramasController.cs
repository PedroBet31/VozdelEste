using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using VozdelEste.Models;

public class ComentariosProgramasController : Controller
{
    private VozDelEsteDBEntities db = new VozDelEsteDBEntities();

    public ActionResult PorPrograma(int? id)
    {
        if (id == null)
            return RedirectToAction("Index", "Programas");

        var programa = db.Programas.Find(id);
        if (programa == null)
            return HttpNotFound();

        var comentarios = db.ComentariosProgramas
            .Where(c => c.ProgramaId == id)
            .Select(c => new ComentarioDetalleViewModel
            {
                Id = c.Id,
                ClienteCI = c.ClienteCI,
                ClienteNombre = c.Clientes.Nombre,
                Comentario = c.Comentario,
                Fecha = c.Fecha,
                ProgramaId = c.ProgramaId
            }).ToList();

        var viewModel = new ComentariosViewModel
        {
            Programa = programa,
            Comentarios = comentarios,
            NuevoComentario = new ComentariosProgramas { ProgramaId = id.Value }
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearComentario(ComentariosProgramas nuevoComentario)
    {
        var usuario = Session["Usuario"] as Usuarios;
        if (usuario == null) return RedirectToAction("Login", "Usuarios");

        var cliente = db.Clientes.FirstOrDefault(c => c.Email == usuario.Email);
        if (cliente == null) return RedirectToAction("Login", "Usuarios");

        nuevoComentario.ClienteCI = cliente.CI;
        nuevoComentario.Fecha = DateTime.Now;

        if (ModelState.IsValid)
        {
            db.ComentariosProgramas.Add(nuevoComentario);
            db.SaveChanges();
        }

        return RedirectToAction("PorPrograma", new { id = nuevoComentario.ProgramaId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarComentario(int id, string comentario)
    {
        var usuario = Session["Usuario"] as Usuarios;
        if (usuario == null)
            return RedirectToAction("Login", "Usuarios");

        var cliente = db.Clientes.FirstOrDefault(c => c.Email == usuario.Email);
        var comentarioDb = db.ComentariosProgramas.Find(id);

        if (comentarioDb == null || cliente == null || comentarioDb.ClienteCI != cliente.CI)
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

        comentarioDb.Comentario = comentario;
        db.SaveChanges();

        return RedirectToAction("PorPrograma", new { id = comentarioDb.ProgramaId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult BorrarComentarioPost(int id)
    {
        var usuario = Session["Usuario"] as Usuarios;
        if (usuario == null)
            return RedirectToAction("Login", "Usuarios");

        var comentario = db.ComentariosProgramas.Find(id);
        if (comentario == null)
            return HttpNotFound();

        var cliente = db.Clientes.FirstOrDefault(c => c.Email == usuario.Email);
        bool esClienteAutor = cliente != null && comentario.ClienteCI == cliente.CI;
        bool esAdmin = usuario.RolId == 1;

        if (!esClienteAutor && !esAdmin)
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

        db.ComentariosProgramas.Remove(comentario);
        db.SaveChanges();

        return RedirectToAction("PorPrograma", new { id = comentario.ProgramaId });
    }
}
