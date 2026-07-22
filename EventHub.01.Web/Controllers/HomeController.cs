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
    }
}
