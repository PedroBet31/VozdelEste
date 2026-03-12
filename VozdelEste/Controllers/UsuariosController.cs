using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using VozdelEste.Models;

namespace VozdelEste.Controllers
{
    public class UsuariosController : BaseController
    {
        private VozDelEsteDBEntities db = new VozDelEsteDBEntities();

        public ActionResult Index()
        {
            if (!TienePermiso("Ver usuarios"))
                return RedirectToAction("Login");

            var usuarios = db.Usuarios.Include("Roles").ToList();
            return View(usuarios);
        }

        public ActionResult Details(int? id)
        {
            if (!TienePermiso("Ver usuarios")) return RedirectToAction("Login");
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var usuario = db.Usuarios.Find(id);
            if (usuario == null) return HttpNotFound();

            return View(usuario);
        }

        public ActionResult Create()
        {
            if (!TienePermiso("Crear usuarios")) return RedirectToAction("Login");

            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nombre,Email,Contrasena,RolId")] Usuarios usuario)
        {
            if (!TienePermiso("Crear usuarios")) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                db.Usuarios.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        public ActionResult Edit(int? id)
        {
            var usuarioSesion = Session["Usuario"] as Usuarios;
            if (usuarioSesion == null) return RedirectToAction("Login");

            if (!TienePermiso("Editar usuarios") && usuarioSesion.Id != id)
                return RedirectToAction("Login");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var usuario = db.Usuarios.Find(id);
            if (usuario == null) return HttpNotFound();

            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Email,RolId")] Usuarios usuario, string ContrasenaNueva, string ConfirmacionContrasena)
        {
            var usuarioSesion = Session["Usuario"] as Usuarios;
            if (usuarioSesion == null)
                return RedirectToAction("Login");

            if (!TienePermiso("Editar usuarios") && usuarioSesion.Id != usuario.Id)
                return RedirectToAction("Login");

            var actual = db.Usuarios.Find(usuario.Id);
            if (actual == null)
                return HttpNotFound();

            if (!string.IsNullOrWhiteSpace(ContrasenaNueva))
            {
                if (ContrasenaNueva != ConfirmacionContrasena)
                {
                    ModelState.AddModelError("", "Las contraseñas no coinciden.");
                    ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
                    return View(usuario);
                }

                actual.Contrasena = ContrasenaNueva;
            }

            actual.Nombre = usuario.Nombre;
            actual.Email = usuario.Email;

            if (TienePermiso("Editar usuarios") && usuarioSesion.Id != usuario.Id)
            {
                actual.RolId = usuario.RolId;
            }

            db.SaveChanges();

            var clienteRelacionado = db.Clientes.FirstOrDefault(c => c.UsuarioId == actual.Id);
            if (clienteRelacionado != null)
            {
                clienteRelacionado.Email = actual.Email;
                db.SaveChanges();
            }

            if (usuarioSesion.Id == usuario.Id)
            {
                var actualizado = db.Usuarios.Include("Roles").FirstOrDefault(u => u.Id == usuario.Id);
                Session["Usuario"] = actualizado;
            }

            return TienePermiso("Ver usuarios") ? RedirectToAction("Index") : RedirectToAction("Perfil");
        }



        public ActionResult Delete(int? id)
        {
            var usuarioSesion = Session["Usuario"] as Usuarios;
            if (usuarioSesion == null) return RedirectToAction("Login");

            if (!TienePermiso("Eliminar usuarios") && usuarioSesion.Id != id)
                return RedirectToAction("Login");

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var usuario = db.Usuarios.Find(id);
            if (usuario == null) return HttpNotFound();

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var usuarioSesion = Session["Usuario"] as Usuarios;
            if (usuarioSesion == null)
                return RedirectToAction("Login");

            if (!TienePermiso("Eliminar usuarios") && usuarioSesion.Id != id)
                return RedirectToAction("Login");

            var usuario = db.Usuarios.Find(id);
            if (usuario == null) return HttpNotFound();

            var cliente = db.Clientes.FirstOrDefault(c => c.UsuarioId == usuario.Id);

            if (cliente != null)
            {
                var comentarios = db.ComentariosProgramas.Where(c => c.ClienteCI == cliente.CI).ToList();
                db.ComentariosProgramas.RemoveRange(comentarios);

                db.Clientes.Remove(cliente);
            }

            db.Usuarios.Remove(usuario);
            db.SaveChanges();

            if (usuarioSesion.Id == id)
                return RedirectToAction("Logout");

            return RedirectToAction("Index");
        }


        public ActionResult Login() => View();

        [HttpPost]
        public ActionResult Login(string email, string contrasena)
        {
            var usuario = db.Usuarios
            .Include(u => u.Roles.Permisos)
            .FirstOrDefault(u => u.Email == email && u.Contrasena == contrasena);

            if (usuario != null)
            {
                Session["Usuario"] = usuario;

                var cliente = db.Clientes.FirstOrDefault(c => c.UsuarioId == usuario.Id);
                if (cliente != null)
                {
                    Session["ClienteCI"] = cliente.CI;
                }

                if (usuario.TienePermiso("Ver dashboard"))
                    return RedirectToAction("Index", "Dashboard");
                if (usuario.TienePermiso("Crear noticias"))
                    return RedirectToAction("Index", "Noticias");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email o contraseña incorrectos.";
            return View();
        }


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        public ActionResult Perfil()
        {
            var usuario = Session["Usuario"] as Usuarios;
            if (usuario == null) return RedirectToAction("Login");

            var usuarioBD = db.Usuarios.Find(usuario.Id);
            if (usuarioBD == null) return HttpNotFound();

            return View(usuarioBD);
        }

        public ActionResult GestionarUsuarios()
        {
            if (!TienePermiso("Editar usuarios")) return RedirectToAction("Login");

            var usuarios = db.Usuarios.Include("Roles").ToList();
            return View(usuarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarRol(int usuarioId, int nuevoRolId)
        {
            if (!TienePermiso("Editar usuarios")) return RedirectToAction("Login");

            var usuario = db.Usuarios.Find(usuarioId);
            if (usuario == null) return HttpNotFound();

            usuario.RolId = nuevoRolId;
            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("GestionarUsuarios");
        }

        public ActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string nombre, string apellido, string ci, DateTime fechaNacimiento, string email, string contrasena, string confirmacion)
        {
            if (contrasena != confirmacion)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View();
            }

            if (ci.Length != 8 || !ci.All(char.IsDigit))
            {
                ViewBag.Error = "La cédula debe tener exactamente 8 dígitos numéricos.";
                return View();
            }

            if (db.Usuarios.Any(u => u.Email == email))
            {
                ViewBag.Error = "Ya existe un usuario con ese correo.";
                return View();
            }

            if (db.Clientes.Any(c => c.CI == ci))
            {
                ViewBag.Error = "Ya existe un cliente con ese CI.";
                return View();
            }

            var rolCliente = db.Roles.FirstOrDefault(r => r.Nombre == "Cliente");
            if (rolCliente == null)
            {
                ViewBag.Error = "No se encontró el rol Cliente.";
                return View();
            }

            var nuevoUsuario = new Usuarios
            {
                Nombre = nombre,
                Email = email,
                Contrasena = contrasena,
                RolId = rolCliente.Id
            };

            db.Usuarios.Add(nuevoUsuario);
            db.SaveChanges();

            var nuevoCliente = new Clientes
            {
                CI = ci,
                Nombre = nombre,
                Apellido = apellido,
                Email = email,
                FechaNacimiento = fechaNacimiento,
                UsuarioId = nuevoUsuario.Id
            };

            db.Clientes.Add(nuevoCliente);
            db.SaveChanges();

            return RedirectToAction("Login");
        }


    }
}
