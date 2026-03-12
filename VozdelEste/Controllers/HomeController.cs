using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VozdelEste.Models;
using VozdelEste.Services;

namespace VozdelEste.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ApiService apiService = new ApiService();

        public async Task<ActionResult> Index()
        {
                ViewBag.Weather = await apiService.GetWeatherDataAsync();

            
                var monedas = await apiService.GetCurrencyDataAsync();
                if (monedas?.Quotes != null)
                {
                    ViewBag.USD = (1 / monedas.Quotes.Uyuusd).ToString("0.0000");
                    ViewBag.EUR = (1 / monedas.Quotes.Uyueur).ToString("0.0000");
                    ViewBag.BRL = (1 / monedas.Quotes.Uyubrl).ToString("0.0000");
                }
                else
                {
                    ViewBag.USD = "N/D";
                    ViewBag.EUR = "N/D";
                    ViewBag.BRL = "N/D";
                }
        

            using (var db = new VozDelEsteDBEntities())
            {
                ViewBag.UltimaNoticia = db.Noticias.OrderByDescending(n => n.FechaPublicacion).FirstOrDefault();
            }

            return View();
        }

        public async Task<JsonResult> GetWeatherJson()
        {

            var weather = await apiService.GetWeatherDataAsync();
            return Json(new
            {
                temperatura = weather.Main.Temp,
                sensacion = weather.Main.FeelsLike,
                humedad = weather.Main.Humidity,
                descripcion = weather.WeatherWeather[0].Description,
                nombre = weather.Name,
                icono = weather.WeatherWeather[0].Icon
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Pronostico()
        {
            if (!TienePermiso("Ver clima")) return RedirectToAction("Login", "Usuarios");

            var weatherNow = await apiService.GetWeatherDataAsync();
            var forecastJo = await apiService.GetForecastDataAsync();

            if (weatherNow != null)
            {
                var hoy = DateTime.Today;

                using (var db = new VozDelEsteDBEntities())
                {
                    if (!db.Clima.Any(c => c.Fecha == hoy))
                    {
                        db.Clima.Add(new Clima
                        {
                            Fecha = hoy,
                            Temperatura = (decimal)weatherNow.Main.Temp,
                            Descripcion = weatherNow.WeatherWeather[0].Description,
                            Icono = weatherNow.WeatherWeather[0].Icon
                        });

                        db.SaveChanges();
                    }
                }
            }

            ViewBag.WeatherNow = weatherNow;
            return View(forecastJo);
        }

        public async Task<JsonResult> GetCotizacionesJson()
        {
            var hoy = DateTime.Today;

            using (var db = new VozDelEsteDBEntities())
            {
                var cotizacionesHoy = db.Cotizaciones.Where(c => c.Fecha == hoy).ToList();

                if (cotizacionesHoy.Count == 3)
                {
                    return Json(new
                    {
                        error = false,
                        usd = cotizacionesHoy.FirstOrDefault(c => c.TipoMoneda == "USD")?.Valor ?? 0,
                        eur = cotizacionesHoy.FirstOrDefault(c => c.TipoMoneda == "EUR")?.Valor ?? 0,
                        brl = cotizacionesHoy.FirstOrDefault(c => c.TipoMoneda == "BRL")?.Valor ?? 0
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            try
            {
                var monedas = await apiService.GetCurrencyDataAsync();
                if (monedas?.Quotes != null)
                {
                    decimal usd = (decimal)(1 / monedas.Quotes.Uyuusd);
                    decimal eur = (decimal)(1 / monedas.Quotes.Uyueur);
                    decimal brl = (decimal)(1 / monedas.Quotes.Uyubrl);

                    using (var db = new VozDelEsteDBEntities())
                    {
                        db.Cotizaciones.Add(new Cotizaciones { Fecha = hoy, TipoMoneda = "USD", Valor = usd });
                        db.Cotizaciones.Add(new Cotizaciones { Fecha = hoy, TipoMoneda = "EUR", Valor = eur });
                        db.Cotizaciones.Add(new Cotizaciones { Fecha = hoy, TipoMoneda = "BRL", Valor = brl });
                        db.SaveChanges();
                    }

                    return Json(new { error = false, usd, eur, brl }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { error = true, mensaje = "monedas.Quotes es null" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Cotizaciones()
        {
            if (!TienePermiso("Ver cotizaciones"))
                return RedirectToAction("Login", "Usuarios");

            var hoy = DateTime.Today;
            decimal usd = 0, eur = 0, brl = 0;

            using (var db = new VozDelEsteDBEntities())
            {
                var cotizaciones = db.Cotizaciones.Where(c => c.Fecha == hoy).ToList();

                if (cotizaciones.Count == 3)
                {
                    usd = cotizaciones.FirstOrDefault(c => c.TipoMoneda == "USD")?.Valor ?? 0;
                    eur = cotizaciones.FirstOrDefault(c => c.TipoMoneda == "EUR")?.Valor ?? 0;
                    brl = cotizaciones.FirstOrDefault(c => c.TipoMoneda == "BRL")?.Valor ?? 0;
                }
                else
                {
                    var monedas = apiService.GetCurrencyDataAsync().Result;
                    if (monedas?.Quotes != null)
                    {
                        usd = (decimal)(1 / monedas.Quotes.Uyuusd);
                        eur = (decimal)(1 / monedas.Quotes.Uyueur);
                        brl = (decimal)(1 / monedas.Quotes.Uyubrl);

                        db.Cotizaciones.Add(new Cotizaciones { Fecha = hoy, TipoMoneda = "USD", Valor = usd });
                        db.Cotizaciones.Add(new Cotizaciones { Fecha = hoy, TipoMoneda = "EUR", Valor = eur });
                        db.Cotizaciones.Add(new Cotizaciones { Fecha = hoy, TipoMoneda = "BRL", Valor = brl });
                        db.SaveChanges();
                    }
                }
            }

            ViewBag.USD = usd.ToString("0.0000");
            ViewBag.EUR = eur.ToString("0.0000");
            ViewBag.BRL = brl.ToString("0.0000");

            return View();
        }



        public ActionResult Historial(string moneda)
        {
            if (!TienePermiso("Ver cotizaciones")) return RedirectToAction("Login", "Usuarios");

            using (var db = new VozDelEsteDBEntities())
            {
                var historial = db.Cotizaciones
                                  .Where(c => c.TipoMoneda == moneda)
                                  .OrderBy(c => c.Fecha)
                                  .ToList();

                ViewBag.Moneda = moneda;
                return View(historial);
            }
        }

        public PartialViewResult Reproductor()
        {

            using (var db = new VozDelEsteDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var ultimoPrograma = db.Programas
                    .OrderByDescending(p => p.Id)
                    .FirstOrDefault();

                return PartialView("Reproductor", ultimoPrograma);
            }
        }


        public ActionResult About() => View();
        public ActionResult Contact() => View();
    }
}
