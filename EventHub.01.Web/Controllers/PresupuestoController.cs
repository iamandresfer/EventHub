using System.Threading.Tasks;
using System.Web.Mvc;
using EventHub._02.Bussines.DTOs;
using EventHub._02.Bussines.Services;
using EventHub._03.Data;

namespace EventHub._01.Web.Controllers
{
    [Authorize]
    public class PresupuestoController : Controller
    {
        private readonly IGastoService _gastoService;
        private readonly IIngresoService _ingresoService;
        private readonly IEventoService _eventoService;

        public PresupuestoController()
        {
            _gastoService = new GastoService();
            _ingresoService = new IngresoService();
            _eventoService = new EventoService(new EventHubContext());
        }

        public async Task<ActionResult> Index(int? eventoId)
        {
            if (!eventoId.HasValue || eventoId.Value == 0)
            {
                var eventos = await _eventoService.GetAllAsync();
                ViewBag.Eventos = eventos;
                return View("Seleccionar");
            }

            var evento = await _eventoService.GetByIdAsync(eventoId.Value);
            if (evento == null) return HttpNotFound();

            ViewBag.Evento = evento;
            ViewBag.Gastos = _gastoService.ObtenerPorEvento(eventoId.Value);
            ViewBag.Resumen = _gastoService.ObtenerResumenPorCategoria(eventoId.Value);
            ViewBag.Ingresos = _ingresoService.ObtenerPorEvento(eventoId.Value);
            ViewBag.ResumenIngresos = _ingresoService.ObtenerResumenPorTipo(eventoId.Value);

            return View(new GastoFormDto { EventoId = eventoId.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(GastoFormDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Complete todos los campos obligatorios.";
                return RedirectToAction("Index", new { eventoId = dto.EventoId });
            }

            var usuario = User.Identity?.Name ?? "Sistema";
            _gastoService.Crear(dto, usuario);
            TempData["Success"] = "Gasto registrado exitosamente.";
            return RedirectToAction("Index", new { eventoId = dto.EventoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, GastoFormDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Complete todos los campos obligatorios.";
                return RedirectToAction("Index", new { eventoId = dto.EventoId });
            }

            _gastoService.Actualizar(id, dto);
            TempData["Success"] = "Gasto actualizado.";
            return RedirectToAction("Index", new { eventoId = dto.EventoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, int eventoId)
        {
            _gastoService.Eliminar(id);
            TempData["Success"] = "Gasto eliminado.";
            return RedirectToAction("Index", new { eventoId = eventoId });
        }

        [HttpGet]
        public ActionResult Obtener(int id)
        {
            var gasto = _gastoService.ObtenerPorId(id);
            if (gasto == null) return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            return Json(new { success = true, data = gasto }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearIngreso(IngresoFormDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Complete todos los campos obligatorios.";
                return RedirectToAction("Index", new { eventoId = dto.EventoId });
            }

            var usuario = User.Identity?.Name ?? "Sistema";
            _ingresoService.Crear(dto, usuario);
            TempData["Success"] = "Ingreso registrado exitosamente.";
            return RedirectToAction("Index", new { eventoId = dto.EventoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarIngreso(int id, IngresoFormDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Complete todos los campos obligatorios.";
                return RedirectToAction("Index", new { eventoId = dto.EventoId });
            }

            _ingresoService.Actualizar(id, dto);
            TempData["Success"] = "Ingreso actualizado.";
            return RedirectToAction("Index", new { eventoId = dto.EventoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarIngreso(int id, int eventoId)
        {
            _ingresoService.Eliminar(id);
            TempData["Success"] = "Ingreso eliminado.";
            return RedirectToAction("Index", new { eventoId = eventoId });
        }

        [HttpGet]
        public ActionResult ObtenerIngreso(int id)
        {
            var ingreso = _ingresoService.ObtenerPorId(id);
            if (ingreso == null) return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            return Json(new { success = true, data = ingreso }, JsonRequestBehavior.AllowGet);
        }
    }
}
