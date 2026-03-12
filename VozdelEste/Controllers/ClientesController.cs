using System.Linq;
using System.Net;
using System.Web.Mvc;
using VozdelEste.Models;
using VozdelEste.Helpers;
using System.Data.Entity;

namespace VozdelEste.Controllers
{
    public class ClientesController : BaseController
    {
        private VozDelEsteDBEntities db = new VozDelEsteDBEntities();

        public ActionResult Index()
        {
            if (!TienePermiso("Ver usuarios") || UsuarioActual.Roles.Nombre != "Administrador")
                return RedirectToAction("Login", "Usuarios");

            var clientes = db.Clientes
                .Where(c => c.UsuarioId != null && c.Usuarios.Roles.Nombre == "Cliente")
                .ToList();

            return View(clientes);
        }

        public ActionResult Details(string id)
        {
            if (!TienePermiso("Ver usuarios") || UsuarioActual.Roles.Nombre != "Administrador")
                return RedirectToAction("Login", "Usuarios");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var cliente = db.Clientes.Find(id);
            if (cliente == null) return HttpNotFound();

            return View(cliente);
        }

        public ActionResult Create()
        {
            if (!TienePermiso("Crear usuarios") || UsuarioActual.Roles.Nombre != "Administrador")
                return RedirectToAction("Login", "Usuarios");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CrearClienteViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (db.Usuarios.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Ya existe un usuario con ese correo.");
                return View(model);
            }

            if (db.Clientes.Any(c => c.CI == model.CI))
            {
                ModelState.AddModelError("CI", "Ya existe un cliente con ese CI.");
                return View(model);
            }

            var rolCliente = db.Roles.FirstOrDefault(r => r.Nombre == "Cliente");

            var nuevoUsuario = new Usuarios
            {
                Nombre = model.Nombre,
                Email = model.Email,
                Contrasena = model.Password,
                RolId = rolCliente.Id
            };

            db.Usuarios.Add(nuevoUsuario);
            db.SaveChanges();

            var nuevoCliente = new Clientes
            {
                CI = model.CI,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                Email = model.Email,
                FechaNacimiento = model.FechaNacimiento,
                UsuarioId = nuevoUsuario.Id
            };

            db.Clientes.Add(nuevoCliente);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id)
        {
            if (!TienePermiso("Editar usuarios") || UsuarioActual.Roles.Nombre != "Administrador")
                return RedirectToAction("Login", "Usuarios");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var cliente = db.Clientes.Find(id);
            if (cliente == null) return HttpNotFound();

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Clientes cliente)
        {
            if (ModelState.IsValid)
            {
                var existente = db.Clientes.Find(cliente.CI);
                if (existente == null)
                    return HttpNotFound();

                existente.Nombre = cliente.Nombre;
                existente.Apellido = cliente.Apellido;
                existente.Email = cliente.Email;

                cliente.UsuarioId = existente.UsuarioId;

                var usuario = db.Usuarios.Find(existente.UsuarioId);
                if (usuario != null)
                {
                    usuario.Email = cliente.Email;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cliente);
        }

        public ActionResult Delete(string id)
        {
            if (!TienePermiso("Eliminar usuarios") || UsuarioActual.Roles.Nombre != "Administrador")
                return RedirectToAction("Login", "Usuarios");

            var cliente = db.Clientes.Find(id);
            if (cliente == null) return HttpNotFound();

            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var cliente = db.Clientes.Find(id);
            if (cliente == null) return HttpNotFound();

            var comentarios = db.ComentariosProgramas.Where(c => c.ClienteCI == cliente.CI).ToList();
            db.ComentariosProgramas.RemoveRange(comentarios);

            var usuario = db.Usuarios.Find(cliente.UsuarioId);
            if (usuario != null)
            {
                db.Usuarios.Remove(usuario);
            }

            db.Clientes.Remove(cliente);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        public ActionResult ComentariosCliente(string ci)
        {
            var cliente = db.Clientes.Find(ci);
            if (cliente == null)
            {
                return HttpNotFound();
            }

            var comentarios = db.ComentariosProgramas
                .Where(c => c.ClienteCI == cliente.CI)
                .Include(c => c.Programas)
                .ToList();

            ViewBag.NombreCliente = cliente.Nombre + " " + cliente.Apellido;
            ViewBag.ClienteCI = cliente.CI;

            return View("ComentariosCliente", comentarios);
        }


    }
}
