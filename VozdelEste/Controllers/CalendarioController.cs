using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VozdelEste.Models;

namespace VozdelEste.Controllers
{
    public class CalendarioController : Controller
    {
        private VozDelEsteDBEntities db = new VozDelEsteDBEntities();

        public ActionResult Index()
        {
            var programas = db.Programas.ToList();
            return View(programas);
        }
    }

}
