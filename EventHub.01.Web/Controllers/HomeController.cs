using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EventHub._02.Bussines.Services;
using EventHub._03.Data;

namespace EventHub._01.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var context = new EventHubContext();
            var eventoService = new EventoService(context);
            var dashboard = await eventoService.GetDashboardAsync();
            return View(dashboard);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ObtenerRecientes()
        {
            var notifService = new NotificacionService();
            var notificaciones = notifService.ObtenerRecientes(15);
            var noLeidas = notifService.ContarNoLeidas();
            return Json(new { success = true, notificaciones = notificaciones, noLeidas = noLeidas }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarcarLeida(int id)
        {
            var notifService = new NotificacionService();
            notifService.MarcarComoLeida(id);
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarcarTodasLeidas()
        {
            var context = new EventHubContext();
            var noLeidas = context.Notificaciones.Where(n => !n.Leida).ToList();
            foreach (var n in noLeidas) n.Leida = true;
            context.SaveChanges();
            return Json(new { success = true });
        }
    }
}
